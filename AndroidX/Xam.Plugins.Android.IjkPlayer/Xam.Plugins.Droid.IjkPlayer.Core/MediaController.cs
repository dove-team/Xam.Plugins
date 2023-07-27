using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System;
using Xam.Plugins.Droid.IjkPlayer.Core.Callback;
using static Android.Widget.SeekBar;

namespace Xam.Plugins.Droid.IjkPlayer.Core
{
    public class MediaController : FrameLayout, IDanmukuSwitchListener, IOnSeekBarChangeListener, IVideoBackListener
    {
        private static int sDefaultTimeout = 3000;
        private static int FADE_OUT = 1;
        private static int SHOW_PROGRESS = 2;
        private Context mContext;
        private PopupWindow mWindow;
        private int mAnimStyle;
        private View mAnchor;
        private View mRoot;
        private SeekBar mProgress;
        private TextView mEndTime, mCurrentTime;
        private TextView mTitleView;
        private OutlineTextView mInfoView;
        private string mTitle;
        private long mDuration;
        private bool mShowing;
        private bool mDragging;
        private bool mInstantSeeking = true;
        private bool mFromXml = false;
        private ImageButton mPauseButton;
        private AudioManager audioManager;
        private IOnShownListener mShownListener;
        private IOnHiddenListener mHiddenListener;
        private bool mDanmakuShow = false;
        private ImageView mBack;
        private ImageView mTvPlay;
        private Handler mHandler;
        private Runnable lastRunnable;
        private IMediaPlayerListener mPlayer;
        private IVideoBackListener mVideoBackListener;
        public void SetVideoBackEvent(IVideoBackListener videoBackListener)
        {
            this.mVideoBackListener = videoBackListener;
        }
        public MediaController(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mRoot = this;
            mFromXml = true;
            InitController(context);
        }
        public MediaController(Context context) : base(context)
        {
            if (!mFromXml && InitController(context))
                InitFloatingWindow();
        }
        private bool InitController(Context context)
        {
            mContext = context;
            audioManager = (AudioManager)mContext.GetSystemService(Context.AudioService);
            mHandler = new Handler((Message msg) =>
            {
                long pos;
                switch (msg.What)
                {
                    case 1:
                        Hide();
                        break;
                    case 2:
                        pos = SetProgress();
                        if (!mDragging && mShowing)
                        {
                            msg = mHandler.ObtainMessage(msg.What);
                            mHandler.SendMessageDelayed(msg, 1000 - (pos % 1000));
                            UpdatePausePlay();
                        }
                        break;
                }
            });
            return true;
        }
        private static string GenerateTime(long position)
        {
            int totalSeconds = (int)((position / 1000.0) + 0.5);
            int seconds = totalSeconds % 60;
            int minutes = (totalSeconds / 60) % 60;
            int hours = totalSeconds / 3600;
            if (hours > 0)
                return string.Format("{0}:{1}:{2}", hours, minutes, seconds);
            else
                return string.Format("{0}:{1}", minutes, seconds);
        }
        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            if (mRoot != null)
                InitControllerView(mRoot);
        }
        private void InitFloatingWindow()
        {
            mWindow = new PopupWindow(mContext);
            mWindow.Focusable = false;
            mWindow.SetBackgroundDrawable(null);
            mWindow.OutsideTouchable = true;
            mAnimStyle = global::Android.Resource.Style.Animation;
        }
        public void SetAnchorView(View view)
        {
            mAnchor = view;
            if (!mFromXml)
            {
                RemoveAllViews();
                mRoot = MakeControllerView();
                mWindow.ContentView = mRoot;
                mWindow.Width = view.Width;
                mWindow.Height = view.Height;
            }
            InitControllerView(mRoot);
        }
        protected View MakeControllerView()
        {
            return LayoutInflater.From(mContext).Inflate(Resource.Layout.layout_media_controller, this, false);
        }
        private void InitControllerView(View v)
        {
            mPauseButton = v.FindViewById<ImageButton>(Resource.Id.media_controller_play_pause);
            mTvPlay = v.FindViewById<ImageView>(Resource.Id.media_controller_tv_play);
            if (mPauseButton != null && mTvPlay != null)
            {
                mPauseButton.RequestFocus();
                mPauseButton.Clickable = true;
                mPauseButton.Click += delegate
                {
                    DoPauseResume();
                    Show(sDefaultTimeout);
                };
                mTvPlay.RequestFocus();
                mTvPlay.Clickable = true;
                mTvPlay.Click += delegate
                {
                    DoPauseResume();
                    Show(sDefaultTimeout);
                };
            }
            mProgress = v.FindViewById<SeekBar>(Resource.Id.media_controller_seekbar);
            if (mProgress != null)
            {
                mProgress.SetOnSeekBarChangeListener(this);
                mProgress.ThumbOffset = 1;
                mProgress.Max = 1000;
            }
            mEndTime = v.FindViewById<TextView>(Resource.Id.media_controller_time_total);
            mCurrentTime = v.FindViewById<TextView>(Resource.Id.media_controller_time_current);
            mTitleView = v.FindViewById<TextView>(Resource.Id.media_controller_title);
            if (mTitleView != null)
            {
                mTitleView.Text = mTitle;
            }
            mBack = v.FindViewById<ImageView>(Resource.Id.media_controller_back);
            mBack.Click += delegate
            {
                DoPauseResume();
                Show(sDefaultTimeout);
                mVideoBackListener.Back();
            };
        }
        public void SetMediaPlayer(IMediaPlayerListener player)
        {
            mPlayer = player;
            UpdatePausePlay();
        }
        public void SetInstantSeeking(bool seekWhenDragging)
        {
            mInstantSeeking = seekWhenDragging;
        }
        public void Show()
        {
            Show(sDefaultTimeout);
        }
        public void SetTitle(string name)
        {
            mTitle = name;
            if (mTitleView != null)
            {
                mTitleView.Text = mTitle; ;
            }
        }
        public void SetInfoView(OutlineTextView v)
        {
            mInfoView = v;
        }
        private void DisableUnsupportedButtons()
        {
            if (mPauseButton != null && mTvPlay != null && !mPlayer.CanPause())
            {
                mPauseButton.Enabled = false;
                mTvPlay.Enabled = false;
            }
        }
        public void SetAnimationStyle(int animationStyle)
        {
            mAnimStyle = animationStyle;
        }
        public void Show(int timeout)
        {
            if (!mShowing && mAnchor != null && mAnchor.WindowToken != null)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
                    mAnchor.SystemUiVisibility = StatusBarVisibility.Visible;
                if (mPauseButton != null && mTvPlay != null)
                {
                    mPauseButton.RequestFocus();
                    mTvPlay.RequestFocus();
                }
                DisableUnsupportedButtons();
                if (mFromXml)
                    mAnchor.Visibility = ViewStates.Visible;
                else
                {
                    int[] location = new int[2];
                    mAnchor.GetLocationOnScreen(location);
                    Rect anchorRect = new Rect(location[0], location[1], location[0] + mAnchor.Width, location[1] + mAnchor.Height);
                    mWindow.AnimationStyle = mAnimStyle;
                    mWindow.ShowAtLocation(mAnchor, GravityFlags.Bottom, anchorRect.Left, 0);
                }
                mShowing = true;
                if (mShownListener != null)
                {
                    mShownListener.OnShown();
                }
            }
            UpdatePausePlay();
            mHandler.SendEmptyMessage(SHOW_PROGRESS);
            if (timeout != 0)
            {
                mHandler.RemoveMessages(FADE_OUT);
                mHandler.SendMessageDelayed(mHandler.ObtainMessage(FADE_OUT), timeout);
            }
        }
        public bool IsShowing()
        {
            return mShowing;
        }
        public void Hide()
        {
            if (mAnchor == null)
                return;
            if (mShowing)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
                    mAnchor.SystemUiVisibility = StatusBarVisibility.Hidden;
                try
                {
                    mHandler.RemoveMessages(SHOW_PROGRESS);
                    if (mFromXml)
                        mAnchor.Visibility = ViewStates.Gone;
                    else
                        mWindow.Dismiss();
                }
                catch { }
                mShowing = false;
                if (mHiddenListener != null)
                    mHiddenListener.OnHidden();
            }
        }
        public void SetOnShownListener(IOnShownListener l)
        {
            mShownListener = l;
        }
        public void SetOnHiddenListener(IOnHiddenListener l)
        {
            mHiddenListener = l;
        }
        private long SetProgress()
        {
            if (mPlayer == null || mDragging)
                return 0;
            int position = mPlayer.GetCurrentPosition();
            int duration = mPlayer.GetDuration();
            if (mProgress != null)
            {
                if (duration > 0)
                    mProgress.Progress = (int)(1000L * position / duration);
                int percent = mPlayer.GetBufferPercentage();
                mProgress.SecondaryProgress = percent * 10;
            }
            mDuration = duration;
            if (mEndTime != null)
                mEndTime.Text = GenerateTime(mDuration);
            if (mCurrentTime != null)
                mCurrentTime.Text = GenerateTime(position);
            return position;
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
            Show(sDefaultTimeout);
            return true;
        }
        public override bool OnTrackballEvent(MotionEvent ev)
        {
            Show(sDefaultTimeout);
            return false;
        }
        public override bool DispatchKeyEvent(KeyEvent e)
        {
            int keyCode = Convert.ToInt32(e.KeyCode);
            if (e.RepeatCount == 0 && (keyCode == (int)Keycode.Headsethook
                    || keyCode == (int)Keycode.MediaPlayPause || keyCode == (int)Keycode.Space))
            {
                DoPauseResume();
                Show(sDefaultTimeout);
                if (mPauseButton != null && mTvPlay != null)
                {
                    mPauseButton.RequestFocus();
                    mTvPlay.RequestFocus();
                }
                return true;
            }
            else if (keyCode == (int)Keycode.MediaStop)
            {
                if (mPlayer.IsPlaying())
                {
                    mPlayer.Pause();
                    UpdatePausePlay();
                }
                return true;
            }
            else if (keyCode == (int)Keycode.Back || keyCode == (int)Keycode.Menu)
            {
                Hide();
                return true;
            }
            else
            {
                Show(sDefaultTimeout);
            }
            return base.DispatchKeyEvent(e);
        }
        private void UpdatePausePlay()
        {
            if (mRoot == null || mPauseButton == null || mTvPlay == null)
                return;
            if (mPlayer.IsPlaying())
            {
                mPauseButton.SetImageResource(Resource.Drawable.bili_player_play_can_pause);
                mTvPlay.SetImageResource(Resource.Drawable.ic_tv_stop);
            }
            else
            {
                mPauseButton.SetImageResource(Resource.Drawable.bili_player_play_can_play);
                mTvPlay.SetImageResource(Resource.Drawable.ic_tv_play);
            }
        }
        private void DoPauseResume()
        {
            if (mPlayer.IsPlaying())
                mPlayer.Pause();
            else
                mPlayer.Start();
            UpdatePausePlay();
        }
        public void SetEnabled(bool enabled)
        {
            if (mPauseButton != null)
                mPauseButton.Enabled = enabled;
            if (mTvPlay != null)
                mTvPlay.Enabled = enabled;
            if (mProgress != null)
                mProgress.Enabled = enabled;
            DisableUnsupportedButtons();
            base.Enabled = enabled;
        }
        public void SetDanmakuShow(bool isShow)
        {
            return;
        }
        public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
        {
            if (!fromUser) return;
            long newposition = (mDuration * progress) / 1000;
            string time = GenerateTime(newposition);
            if (mInstantSeeking)
            {
                mHandler.RemoveCallbacks(lastRunnable);
                lastRunnable = new Runnable(() => mPlayer.SeekTo(newposition));
                mHandler.PostDelayed(lastRunnable, 200);
            }
            if (mInfoView != null)
                mInfoView.Text = time;
            if (mCurrentTime != null)
                mCurrentTime.Text = time;
        }
        public void OnStartTrackingTouch(SeekBar seekBar)
        {
            mDragging = true;
            Show(3600000);
            mHandler.RemoveMessages(SHOW_PROGRESS);
            if (mInstantSeeking)
                audioManager.SetStreamVolume(Stream.Music, (int)Adjust.Mute, 0);
            if (mInfoView != null)
            {
                mInfoView.Text = "";
                mInfoView.Visibility = ViewStates.Visible;
            }
        }
        public void OnStopTrackingTouch(SeekBar seekBar)
        {
            if (!mInstantSeeking)
            {
                mPlayer.SeekTo((mDuration * mProgress.Progress) / 1000);
            }
            if (mInfoView != null)
            {
                mInfoView.Text = "";
                mInfoView.Visibility = ViewStates.Gone;
            }
            Show(sDefaultTimeout);
            mHandler.RemoveMessages(SHOW_PROGRESS);
            audioManager.SetStreamVolume(Stream.Music, (int)Adjust.Unmute, 0);
            mDragging = false;
            mHandler.SendEmptyMessageDelayed(SHOW_PROGRESS, 1000);
        }
        public void Back()
        {
            DoPauseResume();
            Show(sDefaultTimeout);
        }
        public interface IOnShownListener
        {
            void OnShown();
        }
        public interface IOnHiddenListener
        {
            void OnHidden();
        }
    }
}