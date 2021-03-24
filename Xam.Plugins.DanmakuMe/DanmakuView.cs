using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xam.Plugins.DanmakuMe
{
    //https://github.com/LittleFogCat/EasyDanmaku
    public class DanmakuView : TextView
    {
        public Danmaku mDanmaku { get; set; }
        private ListenerInfo mListenerInfo;
        private class ListenerInfo
        {
            public List<OnEnterListener> mOnEnterListeners;
            public List<OnExitListener> mOnExitListener;
        }
        public interface OnEnterListener
        {
            void onEnter(DanmakuView view);
        }
        public interface OnExitListener
        {
            void onExit(DanmakuView view);
        }
        private int mDuration;
        public DanmakuView(Context context) : base(context) { }
        public DanmakuView(Context context, IAttributeSet attrs) : base(context, attrs) { }
        public DanmakuView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { }
        public void setDanmaku(Danmaku danmaku)
        {
            mDanmaku = danmaku;
            SetText(danmaku.text);
            switch (danmaku.mode)
            {
                case Danmaku.Mode.top:
                case Danmaku.Mode.bottom:
                    Gravity = GravityFlags.Center;
                    break;
                case Danmaku.Mode.scroll:
                default:
                    Gravity = GravityFlags.Start | GravityFlags.CenterVertical;
                    break;
            }
        }
        public void show(ViewGroup parent, int duration)
        {
            mDuration = duration;
            switch (mDanmaku.mode)
            {
                case Danmaku.Mode.top:
                case Danmaku.Mode.bottom:
                    showFixedDanmaku(parent, duration);
                    break;
                case Danmaku.Mode.scroll:
                default:
                    showScrollDanmaku(parent, duration);
                    break;
            }
            if (hasOnEnterListener())
            {
                foreach (OnEnterListener listener in getListenerInfo().mOnEnterListeners)
                    listener.onEnter(this);
            }
            PostDelayed(() =>
            {
                Visibility = ViewStates.Gone;
                if (hasOnExitListener())
                {
                    foreach (OnExitListener listener in getListenerInfo().mOnExitListener)
                        listener.onExit(this);
                }
                parent.RemoveView(this);
            }, duration);
        }
        private void showScrollDanmaku(ViewGroup parent, int duration)
        {
            int screenWidth = ScreenUtil.getScreenWidth();
            int textLength = getTextLength();
            ScrollTo(-screenWidth, 0);
            parent.AddView(this);
            smoothScrollTo(textLength, 0, duration);
        }
        private void showFixedDanmaku(ViewGroup parent, int duration)
        {
            Gravity = GravityFlags.Center;
            parent.AddView(this);
        }
        private ListenerInfo getListenerInfo()
        {
            if (mListenerInfo == null)
                mListenerInfo = new ListenerInfo();
            return mListenerInfo;
        }
        public void addOnEnterListener(OnEnterListener l)
        {
            ListenerInfo li = getListenerInfo();
            if (li.mOnEnterListeners == null)
                li.mOnEnterListeners = new List<OnEnterListener>();
            if (!li.mOnEnterListeners.Contains(l))
                li.mOnEnterListeners.Add(l);
        }
        public void clearOnEnterListeners()
        {
            ListenerInfo li = getListenerInfo();
            if (li.mOnEnterListeners == null || li.mOnEnterListeners.Count == 0)
                return;
            li.mOnEnterListeners.Clear();
        }
        public void addOnExitListener(OnExitListener l)
        {
            ListenerInfo li = getListenerInfo();
            if (li.mOnExitListener == null)
                li.mOnExitListener = new List<OnExitListener>();
            if (!li.mOnExitListener.Contains(l))
                li.mOnExitListener.Add(l);
        }
        public void clearOnExitListeners()
        {
            ListenerInfo li = getListenerInfo();
            if (li.mOnExitListener == null || li.mOnExitListener.Count == 0)
                return;
            li.mOnExitListener.Clear();
        }
        public bool hasOnEnterListener()
        {
            ListenerInfo li = getListenerInfo();
            return li.mOnEnterListeners != null && li.mOnEnterListeners.Count != 0;
        }
        public bool hasOnExitListener()
        {
            ListenerInfo li = getListenerInfo();
            return li.mOnExitListener != null && li.mOnExitListener.Count != 0;
        }
        public int getTextLength()
        {
            return (int)Paint.MeasureText(Text.ToString());
        }

        public int getDuration()
        {
            return mDuration;
        }
        private void SetText(object text)
        {
            throw new NotImplementedException();
        }
    }
}