using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Java.IO;
using Java.Lang;
using System.Collections.Generic;
using System.Linq;
using TV.Danmaku.Ijk.Media.Player;
using TV.Danmaku.Ijk.Media.Player.Pragma;
using Xam.Plugins.Droid.IjkPlayer.Core.Callback;
using static Android.Views.ViewGroup;
using Uri = Android.Net.Uri;

namespace Xam.Plugins.Droid.IjkPlayer.Core
{
    public class VideoPlayerView : SurfaceView, IMediaPlayerListener, IMediaPlayerOnCompletionListener, IMediaPlayerOnPreparedListener,
                                  IMediaPlayerOnErrorListener, IMediaPlayerOnSeekCompleteListener, IMediaPlayerOnInfoListener,
                                  IMediaPlayerOnBufferingUpdateListener, IMediaPlayerOnVideoSizeChangedListener, ISurfaceHolderCallback
    {
        public static string TAG = "VideoPlayerView";
        public static int VIDEO_LAYOUT_ORIGIN = 0;
        public static int VIDEO_LAYOUT_SCALE = 1;
        public static int VIDEO_LAYOUT_STRETCH = 2;
        public static int VIDEO_LAYOUT_ZOOM = 3;
        public static int STATE_ERROR = -1;
        public static int STATE_IDLE = 0;
        public static int STATE_PREPARING = 1;
        public static int STATE_PREPARED = 2;
        public static int STATE_PLAYING = 3;
        public static int STATE_PAUSED = 4;
        public static int STATE_PLAYBACK_COMPLETED = 5;
        public static int STATE_SUSPEND = 6;
        public static int STATE_RESUME = 7;
        public static int STATE_SUSPEND_UNSUPPORTED = 8;
        public Uri Uri { get; set; }
        public long Duration;
        public int CurrentState = STATE_IDLE;
        public int TargetState = STATE_IDLE;
        public int VideoLayout = VIDEO_LAYOUT_SCALE;
        public ISurfaceHolder SurfaceHolder = null;
        public IMediaPlayer MediaPlayer = null;
        public int VideoWidth;
        public int VideoHeight;
        public int VideoSarNum;
        public int VideoSarDen;
        public int SurfaceWidth;
        public int SurfaceHeight;
        public MediaController MediaController;
        public View MediaBufferingIndicator;
        public IMediaPlayerOnCompletionListener OnCompletionListener;
        public IMediaPlayerOnPreparedListener OnPreparedListener;
        public IMediaPlayerOnErrorListener OnErrorListener;
        public IMediaPlayerOnSeekCompleteListener OnSeekCompleteListener;
        public IMediaPlayerOnInfoListener OnInfoListener;
        public IMediaPlayerOnBufferingUpdateListener OnBufferingUpdateListener;
        public IMediaPlayerOnVideoSizeChangedListener OnSizeChangedListener;
        public IOnControllerEventsListener OnControllerEventsListener;
        public Dictionary<string, string> Headers { get; set; }
        public int CurrentBufferPercentage { get; private set; }
        public long SeekWhenPrepared { get; private set; }
        public bool CanPause { get; private set; } = true;
        public bool CanSeekBack { get; private set; } = true;
        public bool CanSeekForward { get; private set; } = true;
        public VideoPlayerView(Context context) : base(context)
        {
            InitVideoView();
        }
        public VideoPlayerView(Context context, IAttributeSet attrs) : base(context, attrs, 0)
        {
            InitVideoView();
        }
        public VideoPlayerView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            InitVideoView();
        }
        private void InitVideoView()
        {
            SurfaceHolder = base.Holder;
            VideoWidth = 0;
            VideoHeight = 0;
            VideoSarNum = 0;
            VideoSarDen = 0;
            SurfaceHolder.AddCallback(this);
            Focusable = true;
            FocusableInTouchMode = true;
            RequestFocus();
            CurrentState = STATE_IDLE;
            TargetState = STATE_IDLE;
            if (Context is Activity activity)
                activity.VolumeControlStream = Stream.Music;
        }
        protected bool IsInPlaybackState()
        {
            return (MediaPlayer != null && CurrentState != STATE_ERROR
                    && CurrentState != STATE_IDLE && CurrentState != STATE_PREPARING);
        }
        public void Resume()
        {
            if (SurfaceHolder == null && CurrentState == STATE_SUSPEND)
                TargetState = STATE_RESUME;
            else if (CurrentState == STATE_SUSPEND_UNSUPPORTED)
                OpenVideo();
        }
        public void Release(bool cleartargetstate)
        {
            if (MediaPlayer != null)
            {
                MediaPlayer.Reset();
                MediaPlayer.Release();
                MediaPlayer = null;
                CurrentState = STATE_IDLE;
                if (cleartargetstate)
                    TargetState = STATE_IDLE;
            }
        }
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int width = GetDefaultSize(VideoWidth, widthMeasureSpec);
            int height = GetDefaultSize(VideoHeight, heightMeasureSpec);
            SetMeasuredDimension(width, height);
        }
        public void SetVideoLayout(int layout)
        {
            LayoutParams lp = base.LayoutParameters;
            var res = ScreenResolution.GetResolution(Context);
            int windowWidth = res.FirstOrDefault().Key, windowHeight = res.FirstOrDefault().Value;
            float windowRatio = windowWidth / (float)windowHeight;
            int sarNum = VideoSarNum;
            int sarDen = VideoSarDen;
            if (VideoHeight > 0 && VideoWidth > 0)
            {
                float videoRatio = ((float)(VideoWidth)) / VideoHeight;
                if (sarNum > 0 && sarDen > 0)
                    videoRatio = videoRatio * sarNum / sarDen;
                SurfaceHeight = VideoHeight;
                SurfaceWidth = VideoWidth;
                if (VIDEO_LAYOUT_ORIGIN == layout && SurfaceWidth < windowWidth && SurfaceHeight < windowHeight)
                {
                    lp.Width = (int)(SurfaceHeight * videoRatio);
                    lp.Height = SurfaceHeight;
                }
                else if (layout == VIDEO_LAYOUT_ZOOM)
                {
                    lp.Width = windowRatio > videoRatio ? windowWidth : (int)(videoRatio * windowHeight);
                    lp.Height = windowRatio < videoRatio ? windowHeight : (int)(windowWidth / videoRatio);
                }
                else
                {
                    bool full = layout == VIDEO_LAYOUT_STRETCH;
                    lp.Width = (full || windowRatio < videoRatio) ? windowWidth : (int)(videoRatio * windowHeight);
                    lp.Height = (full || windowRatio > videoRatio) ? windowHeight : (int)(windowWidth / videoRatio);
                }
                Holder.SetFixedSize(SurfaceWidth, SurfaceHeight);
                DebugLog.Dfmt(TAG,
                        "VIDEO: %dx%dx%f[SAR:%d:%d], Surface: %dx%d, LP: %dx%d, Window: %dx%dx%f",
                        VideoWidth, VideoHeight, videoRatio, VideoSarNum,
                        VideoSarDen, SurfaceWidth, SurfaceHeight, lp.Width,
                        lp.Height, windowWidth, windowHeight, windowRatio);
            }
            VideoLayout = layout;
        }
        public bool IsValid
        {
            get
            {
                return (SurfaceHolder != null && SurfaceHolder.Surface.IsValid);
            }
        }
        public void SetVideoPath(string path)
        {
            SetVideoURI(Uri.Parse(path));
        }
        public void SetVideoURI(Uri uri)
        {
            Uri = uri;
            SeekWhenPrepared = 0;
            OpenVideo();
            RequestLayout();
            Invalidate();
        }
        public void StopPlayback()
        {
            if (MediaPlayer != null)
            {
                MediaPlayer.Stop();
                MediaPlayer.Release();
                MediaPlayer = null;
                CurrentState = STATE_IDLE;
                TargetState = STATE_IDLE;
            }
        }
        public void SetOption(int category, string name, string value)
        {
            var player = (MediaPlayer as IjkMediaPlayer);
            player.SetOption(category, name, value);
        }
        public void SetOption(int category, string name, long value)
        {
            var player = (MediaPlayer as IjkMediaPlayer);
            player.SetOption(category, name, value);
        }
        private void OpenVideo()
        {
            if (Uri == null || SurfaceHolder == null)
                return;
            Intent i = new Intent("com.android.music.musicservicecommand");
            i.PutExtra("command", "pause");
            Context.SendBroadcast(i);
            Release(false);
            try
            {
                Duration = -1;
                CurrentBufferPercentage = 0;
                IjkMediaPlayer ijkMediaPlayer = null;
                if (Uri != null)
                {
                    ijkMediaPlayer = new IjkMediaPlayer();
                    ijkMediaPlayer.SetLogEnabled(false);
                    ijkMediaPlayer.SetOption(IjkMediaPlayer.FfpPropvDecoderAvcodec, "analyzemaxduration", 100L);
                    ijkMediaPlayer.SetOption(IjkMediaPlayer.FfpPropvDecoderAvcodec, "probesize", 10240L);
                    ijkMediaPlayer.SetOption(IjkMediaPlayer.FfpPropvDecoderAvcodec, "flush_packets", 1L);
                    ijkMediaPlayer.SetOption(IjkMediaPlayer.IjkLogInfo, "packet-buffering", 0L);
                    ijkMediaPlayer.SetOption(IjkMediaPlayer.IjkLogInfo, "framedrop", 1L);
                    ijkMediaPlayer.SetOption(IjkMediaPlayer.OptCategoryCodec, "skip_loop_filter", "48");
                    ijkMediaPlayer.SetOption(IjkMediaPlayer.OptCategoryCodec, "start-on-prepared", 0);
                    ijkMediaPlayer.SetOption(IjkMediaPlayer.OptCategoryCodec, "http-detect-range-support", 0);
                    ijkMediaPlayer.SetOption(IjkMediaPlayer.OptCategoryCodec, "skip_frame", 0);
                    ijkMediaPlayer.SetOption(IjkMediaPlayer.OptCategoryCodec, "max_cached_duration", 3000);
                    ijkMediaPlayer.SetOption(IjkMediaPlayer.OptCategoryCodec, "infbuf", 1);
                    if (Headers != null && Headers.Count > 0)
                    {
                        foreach (var header in Headers)
                            ijkMediaPlayer.SetOption(IjkMediaPlayer.OptCategoryFormat, header.Key, header.Value);
                    }
                }
                MediaPlayer = ijkMediaPlayer;
                if (MediaPlayer == null) return;
                MediaPlayer.SetOnPreparedListener(this);
                MediaPlayer.SetOnVideoSizeChangedListener(this);
                MediaPlayer.SetOnCompletionListener(this);
                MediaPlayer.SetOnErrorListener(this);
                MediaPlayer.SetOnBufferingUpdateListener(this);
                MediaPlayer.SetOnInfoListener(this);
                MediaPlayer.SetOnSeekCompleteListener(this);
                if (Uri != null)
                    MediaPlayer.SetDataSource(Context, Uri);
                MediaPlayer.SetDisplay(SurfaceHolder);
                MediaPlayer.SetScreenOnWhilePlaying(true);
                MediaPlayer.PrepareAsync();
                CurrentState = STATE_PREPARING;
                AttachMediaController();
            }
            catch (IOException ex)
            {
                DebugLog.E(TAG, "Unable to open content: " + Uri, ex);
                CurrentState = STATE_ERROR;
                TargetState = STATE_ERROR;
                OnErrorListener.OnError(MediaPlayer, (int)MediaError.Unknown, 0);
            }
            catch (IllegalArgumentException ex)
            {
                DebugLog.E(TAG, "Unable to open content: " + Uri, ex);
                CurrentState = STATE_ERROR;
                TargetState = STATE_ERROR;
                OnErrorListener.OnError(MediaPlayer, (int)MediaError.Unknown, 0);
            }
        }
        public void SetMediaController(MediaController controller)
        {
            if (MediaController != null)
                MediaController.Hide();
            MediaController = controller;
            AttachMediaController();
        }
        public void SetMediaBufferingIndicator(View mediaBufferingIndicator)
        {
            if (MediaBufferingIndicator != null)
                MediaBufferingIndicator.Visibility = ViewStates.Gone;
            MediaBufferingIndicator = mediaBufferingIndicator;
        }
        public void AttachMediaController()
        {
            if (MediaPlayer != null && MediaController != null)
            {
                MediaController.SetMediaPlayer(this);
                View anchorView = this.Parent is View ? (View)this.Parent : this;
                MediaController.SetAnchorView(anchorView);
                MediaController.SetEnabled(IsInPlaybackState());
            }
        }
        public override bool OnTouchEvent(MotionEvent ev)
        {
            if (IsInPlaybackState() && MediaController != null)
                ToggleMediaControlsVisiblity();
            return false;
        }
        public override bool OnTrackballEvent(MotionEvent ev)
        {
            if (IsInPlaybackState() && MediaController != null)
                ToggleMediaControlsVisiblity();
            return false;
        }
        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            bool isKeyCodeSupported = keyCode != Keycode.Back && keyCode != Keycode.VolumeUp
                    && keyCode != Keycode.VolumeDown && keyCode != Keycode.Menu
                    && keyCode != Keycode.Call && keyCode != Keycode.Endcall;
            if (IsInPlaybackState() && isKeyCodeSupported && MediaController != null)
            {
                if (keyCode == Keycode.Headsethook || keyCode == Keycode.MediaPlay || keyCode == Keycode.Space)
                {
                    if (MediaPlayer.IsPlaying)
                    {
                        Pause();
                        MediaController.Show();
                    }
                    else
                    {
                        Start();
                        MediaController.Hide();
                    }
                    return true;
                }
                else if (keyCode == Keycode.MediaStop && MediaPlayer.IsPlaying)
                {
                    Pause();
                    MediaController.Show();
                }
                else
                {
                    ToggleMediaControlsVisiblity();
                }
            }
            return base.OnKeyDown(keyCode, e);
        }
        private void ToggleMediaControlsVisiblity()
        {
            if (MediaController.IsShowing())
                MediaController.Hide();
            else
                MediaController.Show();
        }
        public void SetOnPreparedListener(IMediaPlayerOnPreparedListener l)
        {
            this.OnPreparedListener = l;
        }
        public void SetOnCompletionListener(IMediaPlayerOnCompletionListener l)
        {
            OnCompletionListener = l;
        }
        public void SetOnErrorListener(IMediaPlayerOnErrorListener l)
        {
            OnErrorListener = l;
        }
        public void SetOnBufferingUpdateListener(IMediaPlayerOnBufferingUpdateListener l)
        {
            OnBufferingUpdateListener = l;
        }
        public void SetOnSeekCompleteListener(IMediaPlayerOnSeekCompleteListener l)
        {
            OnSeekCompleteListener = l;
        }
        public void SetOnInfoListener(IMediaPlayerOnInfoListener l)
        {
            OnInfoListener = l;
        }
        public void SetOnControllerEventsListener(IOnControllerEventsListener l)
        {
            OnControllerEventsListener = l;
        }
        public void Start()
        {
            if (IsInPlaybackState())
            {
                MediaPlayer.Start();
                CurrentState = STATE_PLAYING;
            }
            TargetState = STATE_PLAYING;
            OnControllerEventsListener.OnVideoResume();
        }
        public void Pause()
        {
            if (IsInPlaybackState())
            {
                if (MediaPlayer.IsPlaying)
                {
                    MediaPlayer.Pause();
                    CurrentState = STATE_PAUSED;
                }
            }
            TargetState = STATE_PAUSED;
            OnControllerEventsListener.OnVideoPause();
        }
        public int GetDuration()
        {
            if (IsInPlaybackState())
            {
                if (Duration > 0)
                    return (int)Duration;
                Duration = MediaPlayer.Duration;
                return (int)Duration;
            }
            Duration = -1;
            return (int)Duration;
        }
        public int GetCurrentPosition()
        {
            if (IsInPlaybackState())
                return (int)MediaPlayer.CurrentPosition; ;
            return 0;
        }
        public void SeekTo(long pos)
        {
            if (IsInPlaybackState())
            {
                MediaPlayer.SeekTo(pos);
                SeekWhenPrepared = 0;
            }
            else
            {
                SeekWhenPrepared = pos;
            }
        }
        public bool IsPlaying()
        {
            return IsInPlaybackState() && MediaPlayer.IsPlaying;
        }
        public int GetBufferPercentage()
        {
            if (MediaPlayer != null)
                return CurrentBufferPercentage;
            return 0;
        }
        public void OnCompletion(IMediaPlayer p0)
        {
            DebugLog.D(TAG, "OnCompletion");
            CurrentState = STATE_PLAYBACK_COMPLETED;
            TargetState = STATE_PLAYBACK_COMPLETED;
            if (MediaController != null)
                MediaController.Hide();
            if (OnCompletionListener != null)
                OnCompletionListener.OnCompletion(MediaPlayer);
        }
        public void OnPrepared(IMediaPlayer mp)
        {
            DebugLog.Dfmt(TAG, "OnPrepared");
            CurrentState = STATE_PREPARED;
            TargetState = STATE_PLAYING;
            if (OnPreparedListener != null)
                OnPreparedListener.OnPrepared(MediaPlayer);
            if (MediaController != null)
                MediaController.SetEnabled(true);
            VideoWidth = mp.VideoWidth;
            VideoHeight = mp.VideoHeight;
            if (SeekWhenPrepared != 0)
                SeekTo(SeekWhenPrepared);
            if (VideoWidth != 0 && VideoHeight != 0)
            {
                SetVideoLayout(VideoLayout);
                if (SurfaceWidth == VideoWidth && SurfaceHeight == VideoHeight)
                {
                    if (TargetState == STATE_PLAYING)
                    {
                        Start();
                        if (MediaController != null)
                            MediaController.Show();
                    }
                    else if (!IsPlaying() && (SeekWhenPrepared != 0 || GetCurrentPosition() > 0))
                    {
                        if (MediaController != null)
                            MediaController.Show(0);
                    }
                }
            }
            else if (TargetState == STATE_PLAYING)
            {
                Start();
            }
        }
        public bool OnError(IMediaPlayer p0, int what, int extra)
        {
            DebugLog.Dfmt(TAG, "Error: %d, %d", what, extra);
            CurrentState = STATE_ERROR;
            TargetState = STATE_ERROR;
            if (MediaController != null)
                MediaController.Hide();
            if (OnErrorListener != null)
            {
                if (OnErrorListener.OnError(MediaPlayer, what, extra))
                    return true;
            }
            if (WindowToken != null)
            {
                int message = what == (int)MediaError.NotValidForProgressivePlayback ?
                        Resource.String.video_error_text_invalid_progressive_playback : Resource.String.video_error_text_unknown;
                new AlertDialog.Builder(Context)
                        .SetTitle(Resource.String.video_error_title)
                        .SetMessage(message)
                        .SetPositiveButton(Resource.String.video_error_button, (dialog, whichButton) =>
                        {
                            if (OnCompletionListener != null)
                                OnCompletionListener.OnCompletion(MediaPlayer);
                        }).SetCancelable(false).Show();
            }
            return true;
        }
        public void OnSeekComplete(IMediaPlayer mp)
        {
            DebugLog.D(TAG, "onSeekComplete");
            if (OnSeekCompleteListener != null)
                OnSeekCompleteListener.OnSeekComplete(mp);
        }
        public bool OnInfo(IMediaPlayer mp, int what, int extra)
        {
            DebugLog.Dfmt(TAG, "onInfo: (%d, %d)", what, extra);
            if (OnInfoListener != null)
                OnInfoListener.OnInfo(mp, what, extra);
            else if (MediaPlayer != null)
            {
                if (what == (int)global::Android.Media.MediaInfo.BufferingStart)
                {
                    DebugLog.Dfmt(TAG, "onInfo: (MEDIA_INFO_BUFFERING_START)");
                    if (MediaBufferingIndicator != null)
                        MediaBufferingIndicator.Visibility = ViewStates.Visible;
                }
                else if (what == (int)global::Android.Media.MediaInfo.BufferingEnd)
                {
                    DebugLog.Dfmt(TAG, "onInfo: (MEDIA_INFO_BUFFERING_END)");
                    if (MediaBufferingIndicator != null)
                        MediaBufferingIndicator.Visibility = ViewStates.Gone;
                }
            }
            return true;
        }
        public void OnVideoSizeChanged(IMediaPlayer mp, int width, int height, int sarNum, int sarDen)
        {
            DebugLog.Dfmt(TAG, "onVideoSizeChanged: (%dx%d)", width, height);
            VideoWidth = mp.VideoWidth;
            VideoHeight = mp.VideoHeight;
            VideoSarNum = sarNum;
            VideoSarDen = sarDen;
            if (VideoWidth != 0 && VideoHeight != 0)
                SetVideoLayout(VideoLayout);
        }
        public void OnBufferingUpdate(IMediaPlayer mp, int percent)
        {
            CurrentBufferPercentage = percent;
            if (OnBufferingUpdateListener != null)
                OnBufferingUpdateListener.OnBufferingUpdate(mp, percent);
        }
        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            SurfaceHolder = holder;
            if (MediaPlayer != null)
                MediaPlayer.SetDisplay(SurfaceHolder);
            SurfaceWidth = width;
            SurfaceHeight = height;
            bool isValidState = (TargetState == STATE_PLAYING);
            bool hasValidSize = (VideoWidth == width && VideoHeight == height);
            if (MediaPlayer != null && isValidState && hasValidSize)
            {
                if (SeekWhenPrepared != 0)
                    SeekTo(SeekWhenPrepared);
                Start();
                if (MediaController != null)
                {
                    if (MediaController.IsShowing())
                        MediaController.Hide();
                    MediaController.Show();
                }
            }
        }
        public void SurfaceCreated(ISurfaceHolder holder)
        {
            SurfaceHolder = holder;
            if (MediaPlayer != null && CurrentState == STATE_SUSPEND && TargetState == STATE_RESUME)
            {
                MediaPlayer.SetDisplay(SurfaceHolder);
                Resume();
            }
            else
            {
                OpenVideo();
            }
        }
        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            SurfaceHolder = null;
            if (MediaController != null)
                MediaController.Hide();
            if (CurrentState != STATE_SUSPEND)
                Release(true);
        }
        bool IMediaPlayerListener.CanPause()
        {
            return CanPause;
        }
        public interface IOnControllerEventsListener
        {
            void OnVideoPause();
            void OnVideoResume();
        }
    }
}