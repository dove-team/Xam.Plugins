using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using Xam.Plugins.DanmakuMe.Utils;
using Config = Xam.Plugins.DanmakuMe.Utils.Config;

namespace Xam.Plugins.DanmakuMe
{
    public class DanmakuManager
    {
        private const string TAG = "DanmakuManager";
        private const int RESULT_OK = 0;
        private const int RESULT_NULL_ROOT_VIEW = 1;
        private const int RESULT_FULL_POOL = 2;
        private const int TOO_MANY_DANMAKU = 2;
        private static DanmakuManager sInstance;
        /// <summary>
        /// 弹幕容器
        /// </summary>
       internal  WeakReference DanmakuContainer { get; set; }
        /// <summary>
        /// 弹幕池
        /// </summary>
        private IPool<DanmakuView> DanmakuViewPool;
        private Config config;
        public Config Config
        {
            get
            {
                if (config == null)
                    config = new Config();
                return config;
            }
            set
            {
                config = value;
            }
        }
        private DanmakuPositionCalculator PositionCal;
        private DanmakuManager() { }
        public static DanmakuManager Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new DanmakuManager();
                return sInstance;
            }
        }
        /// <summary>
        /// 初始化。在使用之前必须调用该方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="container"></param>
        public void Init(Context context, FrameLayout container)
        {
            if (DanmakuViewPool == null)
            {
                DanmakuViewPool = new CachedDanmakuViewPool(
                        60000, // 缓存存活时间：60秒
                        100, // 最大弹幕数：100
                        new DanmakuViewCreator(() => DanmakuViewFactory.CreateDanmakuView(context, container)));
            }
            SetDanmakuContainer(container);
            ScreenUtil.Init(context);
            Config = new Config();
            PositionCal = new DanmakuPositionCalculator(this);
        }
        private DanmakuPositionCalculator PositionCalculator
        {
            get
            {
                if (PositionCal == null)
                    PositionCal = new DanmakuPositionCalculator(this);
                return PositionCal;
            }
        }
        public void SetDanmakuViewPool(IPool<DanmakuView> pool)
        {
            if (DanmakuViewPool != null)
                DanmakuViewPool.Release();
            DanmakuViewPool = pool;
        }
        /// <summary>
        /// 设置允许同时出现最多的弹幕数，如果屏幕上显示的弹幕数超过该数量，那么新出现的弹幕将被丢弃，直到有旧的弹幕消失。
        /// </summary>
        /// <param name="max">同时出现的最多弹幕数，-1无限制</param>
        public void SetMaxDanmakuSize(int max)
        {
            if (DanmakuViewPool == null)
                return;
            DanmakuViewPool.MaxSize = max;
        }
        /// <summary>
        /// 设置弹幕的容器，所有的弹幕都在这里面显示
        /// </summary>
        /// <param name="FrameLayout"></param>
        /// <param name=""></param>
        public void SetDanmakuContainer(FrameLayout root)
        {
            if (root == null)
                throw new ArgumentNullException("Danmaku container cannot be null!");
            DanmakuContainer = new WeakReference(root);
        }
        /// <summary>
        /// 发送一条弹幕
        /// </summary>
        public int Send(Danmaku danmaku)
        {
            if (DanmakuViewPool == null)
                throw new ArgumentNullException("Danmaku view pool is null. Did you call init() first?");
            DanmakuView view = DanmakuViewPool.Get();
            if (view == null)
            {
                MeVisual.Warn(TAG, "show: Too many danmaku, discard");
                return RESULT_FULL_POOL;
            }
            if (DanmakuContainer == null || DanmakuContainer.Get() == null)
            {
                MeVisual.Warn(TAG, "show: Root view is null.");
                return RESULT_NULL_ROOT_VIEW;
            }
            view.SetDanmaku(danmaku);
            // 字体大小
            int textSize = danmaku.Size;
            view.SetTextSize(ComplexUnitType.Px, textSize);
            // 字体颜色
            try
            {
                var color = Color.ParseColor(danmaku.Color);
                view.SetTextColor(color);
            }
            catch
            {
                view.SetTextColor(Color.White);
            }
            // 计算弹幕距离顶部的位置
            int marginTop = PositionCalculator.GetMarginTop(view);
            if (marginTop == -1)
            {
                // 屏幕放不下了
                MeVisual.Debug(TAG, "send: screen is full, too many danmaku [" + danmaku + "]");
                return TOO_MANY_DANMAKU;
            }
            FrameLayout.LayoutParams p = (FrameLayout.LayoutParams)view.LayoutParameters;
            if (p == null)
            {
                p = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            }
            p.TopMargin = marginTop;
            view.LayoutParameters = p;
            view.SetMinHeight((int)(Config.LineHeight * 1.35));
            view.Show(DanmakuContainer.Get(), GetDisplayDuration(danmaku));
            return RESULT_OK;
        }
        /// <summary>
        /// 返回这个弹幕显示时长
        /// </summary>
        /// <returns></returns>
       internal int GetDisplayDuration(Danmaku danmaku)
        {
            var duration = danmaku.Mode switch
            {
                Danmaku.DanmaukuMode.Top => Config.DurationTop,
                Danmaku.DanmaukuMode.Bottom => Config.DurationBottom,
                _ => Config.DurationScroll,
            };
            return duration;
        }
    }
}