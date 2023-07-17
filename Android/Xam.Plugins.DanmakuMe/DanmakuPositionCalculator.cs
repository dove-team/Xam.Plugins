using Android.Views;
using System.Collections.Generic;
using System.Linq;
using Xam.Plugins.DanmakuMe.Listener;

namespace Xam.Plugins.DanmakuMe
{
    class DanmakuPositionCalculator
    {
        private bool[] Tops { get; }
        private bool[] Bottoms { get; }
        /// <summary>
        /// 保存每一行最后一个弹幕消失的时间
        /// </summary>
        private List<DanmakuView> LastDanmakus { get; }
        private DanmakuManager DanmakuManager { get; }
        public DanmakuPositionCalculator(DanmakuManager danmakuManager)
        {
            DanmakuManager = danmakuManager;
            int maxLine = danmakuManager.Config.GetMaxDanmakuLine();
            Tops = new bool[maxLine];
            Bottoms = new bool[maxLine];
            LastDanmakus = new List<DanmakuView>();
        }
        private int LineHeightWithPadding
        {
            get
            {
                return (int)(1.35f * DanmakuManager.Config.LineHeight);
            }
        }
        internal int GetMarginTop(DanmakuView view)
        {
            return view.Danmaku.Mode switch
            {
                Danmaku.DanmaukuMode.Scroll => GetScrollY(view),
                Danmaku.DanmaukuMode.Top => GetTopY(view),
                Danmaku.DanmaukuMode.Bottom => GetBottomY(view),
                _ => -1,
            };
        }
        private int GetScrollY(DanmakuView view)
        {
            if (LastDanmakus.Count == 0)
            {
                LastDanmakus.Add(view);
                return 0;
            }
            int i;
            for (i = 0; i < LastDanmakus.Count; i++)
            {
                DanmakuView last = LastDanmakus.ElementAt(i);
                int timeDisappear = CalTimeDisappear(last); // 最后一条弹幕还需多久消失
                int timeArrive = CalTimeArrive(view); // 这条弹幕需要多久到达屏幕边缘
                bool isFullyShown = IsFullyShown(last);
                if (timeDisappear <= timeArrive && isFullyShown)
                {
                    // 如果最后一个弹幕在这个弹幕到达之前消失，并且最后一个字已经显示完毕，
                    // 那么新的弹幕就可以在这一行显示
                    LastDanmakus.Insert(i, view);
                    return i * LineHeightWithPadding;
                }
            }
            int maxLine = DanmakuManager.Config.GetMaxDanmakuLine();
            if (maxLine == 0 || i < maxLine)
            {
                LastDanmakus.Add(view);
                return i * LineHeightWithPadding;
            }
            return -1;
        }
        private int GetTopY(DanmakuView view)
        {
            for (int i = 0; i < Tops.Length; i++)
            {
                bool isShowing = Tops[i];
                if (!isShowing)
                {
                    int finalI = i;
                    Tops[finalI] = true;
                    view.AddOnExitListener(new ExitListener(() => Tops[finalI] = false));
                    return i * LineHeightWithPadding;
                }
            }
            return -1;
        }
        private int GetBottomY(DanmakuView view)
        {
            for (int i = 0; i < Bottoms.Length; i++)
            {
                bool isShowing = Bottoms[i];
                if (!isShowing)
                {
                    int finalI = i;
                    Bottoms[finalI] = true;
                    view.AddOnExitListener(new ExitListener(() => Bottoms[finalI] = false));
                    return ParentHeight - (i + 1) * LineHeightWithPadding;
                }
            }
            return -1;
        }
        /// <summary>
        /// 这条弹幕是否已经全部出来了。如果没有的话，后面的弹幕不能出来，否则就重叠了。
        /// </summary>
        private bool IsFullyShown(DanmakuView view)
        {
            if (view == null)
                return true;
            int scrollX = view.ScrollX;
            int textLength = view.TextLength;
            return textLength - scrollX < ParentWidth;
        }
        /// <summary>
        /// 这条弹幕还有多少毫秒彻底消失
        /// </summary>
        private int CalTimeDisappear(DanmakuView view)
        {
            if (view == null)
                return 0;
            float speed = CalSpeed(view);
            int scrollX = view.ScrollX;
            int textLength = view.TextLength;
            int wayToGo = textLength - scrollX;
            return (int)(wayToGo / speed);
        }
        /// <summary>
        /// 这条弹幕还要多少毫秒抵达屏幕边缘。
        /// </summary>
        private int CalTimeArrive(DanmakuView view)
        {
            float speed = CalSpeed(view);
            return (int)(ParentWidth / speed);
        }
        /// <summary>
        /// 这条弹幕的速度。单位：px/ms
        /// </summary>
        private float CalSpeed(DanmakuView view)
        {
            int textLength = view.TextLength;
            float s = textLength + ParentWidth + 0.0f;
            int t = DanmakuManager.GetDisplayDuration(view.Danmaku);
            return s / t;
        }
        private int ParentHeight
        {
            get
            {
                ViewGroup parent = DanmakuManager.DanmakuContainer.Get();
                if (parent == null || parent.Height == 0)
                    return 1080;
                return parent.Height;
            }
        }
        private int ParentWidth
        {
            get
            {
                ViewGroup parent = DanmakuManager.DanmakuContainer.Get();
                if (parent == null || parent.Width == 0)
                    return 1920;
                return parent.Width;
            }
        }
    }
}