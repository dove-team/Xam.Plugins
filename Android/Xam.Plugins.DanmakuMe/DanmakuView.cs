using Android.Content;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System.Collections.Generic;
using Xam.Plugins.DanmakuMe.Interface;
using Xam.Plugins.DanmakuMe.Listener;
using Xam.Plugins.DanmakuMe.Utils;

namespace Xam.Plugins.DanmakuMe
{
    //https://github.com/LittleFogCat/EasyDanmaku
    public class DanmakuView : TextView
    {
        public int Duration { get; set; }
        private Scroller Scroller { get; set; }
        public Danmaku Danmaku { get; set; }
        private ListenerInfo ListenerInfo { get; set; }
        public DanmakuView(Context context) : base(context) { }
        public DanmakuView(Context context, IAttributeSet attrs) : base(context, attrs) { }
        public DanmakuView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { }
        public void SetDanmaku(Danmaku danmaku)
        {
            Danmaku = danmaku;
            Text = danmaku.Text;
            switch (danmaku.Mode)
            {
                case Danmaku.DanmaukuMode.Top:
                case Danmaku.DanmaukuMode.Bottom:
                    Gravity = GravityFlags.Center;
                    break;
                case Danmaku.DanmaukuMode.Scroll:
                default:
                    Gravity = GravityFlags.Start | GravityFlags.CenterVertical;
                    break;
            }
        }
        public void Show(ViewGroup parent, int duration)
        {
            Duration = duration;
            switch (Danmaku.Mode)
            {
                case Danmaku.DanmaukuMode.Top:
                case Danmaku.DanmaukuMode.Bottom:
                    ShowFixedDanmaku(parent);
                    break;
                case Danmaku.DanmaukuMode.Scroll:
                default:
                    ShowScrollDanmaku(parent, duration);
                    break;
            }
            if (HasOnEnterListener)
            {
                foreach (IOnEnterListener listener in CurrentListenerInfo.OnEnterListeners)
                    listener.OnEnter(this);
            }
            PostDelayed(() =>
            {
                Visibility = ViewStates.Gone;
                if (HasOnExitListener)
                {
                    foreach (IOnExitListener listener in CurrentListenerInfo.OnExitListeners)
                        listener.OnExit(this);
                }
                parent.RemoveView(this);
            }, duration);
        }
        private void ShowScrollDanmaku(ViewGroup parent, int duration)
        {
            int screenWidth = ScreenUtil.ScreenWidth;
            ScrollTo(-screenWidth, 0);
            parent.AddView(this);
            SmoothScrollTo(TextLength, 0, duration);
        }
        public void SmoothScrollTo(int x, int y, int duration)
        {
            if (Scroller == null)
            {
                Scroller = new Scroller(Context, new LinearInterpolator());
                SetScroller(Scroller);
            }
            int sx = ScrollX, sy = ScrollY;
            Scroller.StartScroll(sx, sy, x - sx, y - sy, duration);
        }
        private void ShowFixedDanmaku(ViewGroup parent)
        {
            Gravity = GravityFlags.Center;
            parent.AddView(this);
        }
        private ListenerInfo CurrentListenerInfo
        {
            get
            {
                if (ListenerInfo == null)
                    ListenerInfo = new ListenerInfo();
                return ListenerInfo;
            }
        }
        public void AddOnEnterListener(IOnEnterListener l)
        {
            ListenerInfo li = CurrentListenerInfo;
            if (li.OnEnterListeners == null)
                li.OnEnterListeners = new List<IOnEnterListener>();
            if (!li.OnEnterListeners.Contains(l))
                li.OnEnterListeners.Add(l);
        }
        public void ClearOnEnterListeners()
        {
            ListenerInfo li = CurrentListenerInfo;
            if (li.OnEnterListeners == null || li.OnEnterListeners.Count == 0)
                return;
            li.OnEnterListeners.Clear();
        }
        public void AddOnExitListener(IOnExitListener l)
        {
            ListenerInfo li = CurrentListenerInfo;
            if (li.OnExitListeners == null)
                li.OnExitListeners = new List<IOnExitListener>();
            if (!li.OnExitListeners.Contains(l))
                li.OnExitListeners.Add(l);
        }
        public void ClearOnExitListeners()
        {
            ListenerInfo li = CurrentListenerInfo;
            if (li.OnExitListeners == null || li.OnExitListeners.Count == 0)
                return;
            li.OnExitListeners.Clear();
        }
        public bool HasOnEnterListener
        {
            get
            {
                ListenerInfo li = CurrentListenerInfo;
                return li.OnEnterListeners != null && li.OnEnterListeners.Count != 0;
            }
        }
        public bool HasOnExitListener
        {
            get
            {
                ListenerInfo li = CurrentListenerInfo;
                return li.OnExitListeners != null && li.OnExitListeners.Count != 0;
            }
        }
        public int TextLength
        {
            get
            {
                return (int)Paint.MeasureText(Text.ToString());
            }
        }
        public void Restore()
        {
            ClearOnEnterListeners();
            ClearOnExitListeners();
            Visibility = ViewStates.Visible;
            ScrollX = 0;
            ScrollY = 0;
        }
        public override void ComputeScroll()
        {
            if (Scroller != null && Scroller.ComputeScrollOffset())
            {
                ScrollTo(Scroller.CurrX, Scroller.CurrY);
                PostInvalidate();
            }
        }
    }
}