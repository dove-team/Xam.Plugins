using Android.App;
using Android.OS;
using Android.Runtime;
using Xamarin.Essentials;
using Xam.Plugins.Droid.IjkPlayer.Core;
using Xam.Plugins.Droid.IjkPlayer.Core.Activities;
using Xam.Plugins.Droid.IjkPlayer.Core.Callback;
using TV.Danmaku.Ijk.Media.Player;
using Android.Util;
using Xam.Plugins.Droid.IjkPlayer.Core.Utils;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Widget;
using static Android.Widget.TextView;
using MediaController = Xam.Plugins.Droid.IjkPlayer.Core.MediaController;

namespace Demo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : RxBaseActivity, IVideoBackListener, IDanmukuSwitchListener, IMediaPlayerOnInfoListener,
        IMediaPlayerOnSeekCompleteListener, VideoPlayerView.IOnControllerEventsListener, IMediaPlayerOnCompletionListener,
        IMediaPlayerOnPreparedListener
    {
        private const string TAG = "VideoPlayerActivity";
        VideoPlayerView mPlayerView;
        View mBufferingIndicator;
        RelativeLayout mVideoPrepareLayout;
        ImageView mAnimImageView;
        TextView mPrepareText;
        private int cid;
        private string title;
        private int LastPosition = 0;
        private string startText = "初始化播放器...";
        private AnimationDrawable mLoadingAnim;
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public override int GetLayoutId()
        {
            return Resource.Layout.activity_player;
        }
        public override void InitViews(Bundle savedInstanceState)
        {
            try
            {
                if (Intent != null)
                {
                    cid = Intent.GetIntExtra(ConstantUtil.EXTRA_CID, 0);
                    title = "Test";
                }
                InitAnimation();
                InitMediaPlayer();
            }
            catch (Java.Lang.Exception e)
            {
                Log.Error(TAG, "initViews--" + e.ToString());
            }
        }
        public override void LoadData()
        {
            try
            {
                var videoPath = Android.Net.Uri.Parse("http://vfx.mtime.cn/Video/2019/02/04/mp4/190204084208765161.mp4");
                mPlayerView.SetVideoURI(videoPath);
                mPlayerView.SetOnPreparedListener(this);
                mPlayerView.Start();
            }
            catch (Java.Lang.Exception e)
            {
                Log.Error(TAG, "LoadDate--" + e.ToString());
            }
        }
        private void InitAnimation()
        {
            try
            {
                if (mVideoPrepareLayout == null)
                {
                    mPlayerView = FindViewById<VideoPlayerView>(Resource.Id.playerView);
                    mBufferingIndicator = FindViewById<View>(Resource.Id.buffering_indicator);
                    mVideoPrepareLayout = FindViewById<RelativeLayout>(Resource.Id.video_start);
                    mAnimImageView = FindViewById<ImageView>(Resource.Id.bili_anim);
                    mPrepareText = FindViewById<TextView>(Resource.Id.video_start_info);
                }
                mVideoPrepareLayout.Visibility = ViewStates.Visible;
                startText += "【完成】\n解析视频地址...";
                mPrepareText.SetText(startText, BufferType.Normal);
                mLoadingAnim = (AnimationDrawable)mAnimImageView.Background;
                mLoadingAnim.Start();
            }
            catch (Java.Lang.Exception e)
            {
                Log.Error(TAG, "InitAnimation--" + e.ToString());
            }
        }
        private void InitMediaPlayer()
        {
            try
            {
                //配置播放器
                MediaController mMediaController = new MediaController(this);
                mMediaController.SetTitle(title);
                mPlayerView.SetVideoLayout(mPlayerView.Id);
                mPlayerView.SetMediaController(mMediaController);
                mPlayerView.SetMediaBufferingIndicator(mBufferingIndicator);
                mPlayerView.RequestFocus();
                mPlayerView.SetOnInfoListener(this);
                mPlayerView.SetOnSeekCompleteListener(this);
                mPlayerView.SetOnCompletionListener(this);
                mPlayerView.SetOnControllerEventsListener(this);
                //设置返回键监听
                mMediaController.SetVideoBackEvent(this);
                LoadData();
            }
            catch (Java.Lang.Exception e)
            {
                Log.Error(TAG, "InitMediaPlayer--" + e.ToString());
            }
        }
        public override void InitToolBar()
        {
            try
            {
                Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
                Window.SetBackgroundDrawable(null);
                Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            }
            catch (Java.Lang.Exception e)
            {
                Log.Error(TAG, "initToolBar--" + e.ToString());
            }
        }
        public void OnSeekComplete(IMediaPlayer mp)
        {
            return;
        }
        public void OnPrepared(IMediaPlayer mp)
        {
            mLoadingAnim.Stop();
            startText += "【完成】\n视频缓冲中...";
            mPrepareText.SetText(startText, BufferType.Normal);
            mVideoPrepareLayout.Visibility = ViewStates.Gone;
        }
        public void OnVideoPause()
        {
            return;
        }
        public void OnVideoResume()
        {
            return;
        }
        public void Back()
        {
            OnBackPressed();
        }
        public void SetDanmakuShow(bool isShow)
        {
            return;
        }
        public bool OnInfo(IMediaPlayer mp, int what, int extra)
        {
            if (what == (int)Android.Media.MediaInfo.BufferingStart)
            {
                if (mBufferingIndicator != null)
                    mBufferingIndicator.Visibility = ViewStates.Visible;
            }
            else if (what == (int)Android.Media.MediaInfo.BufferingEnd)
            {
                if (mBufferingIndicator != null)
                    mBufferingIndicator.Visibility = ViewStates.Gone;
            }
            return true;
        }
        public void OnCompletion(IMediaPlayer mp)
        {
            mPlayerView.Pause();
        }
        protected override void OnResume()
        {
            base.OnResume();
            if (mPlayerView != null && !mPlayerView.IsPlaying())
                mPlayerView.SeekTo(LastPosition);
        }
        protected override void OnPause()
        {
            base.OnPause();
            if (mPlayerView != null)
                LastPosition = mPlayerView.GetCurrentPosition();
            mPlayerView.Pause();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mPlayerView != null && mPlayerView.DrawingCacheEnabled)
                mPlayerView.DestroyDrawingCache();
            if (mLoadingAnim != null)
            {
                mLoadingAnim.Stop();
                mLoadingAnim = null;
            }
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            if (mLoadingAnim != null)
            {
                mLoadingAnim.Stop();
                mLoadingAnim = null;
            }
        }
    }
}