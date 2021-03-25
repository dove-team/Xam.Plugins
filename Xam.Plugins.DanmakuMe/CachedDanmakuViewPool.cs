using Java.Lang;
using Java.Util.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Xam.Plugins.DanmakuMe.Interface;
using Xam.Plugins.DanmakuMe.Listener;
using Xam.Plugins.DanmakuMe.Utils;

namespace Xam.Plugins.DanmakuMe
{
    public class CachedDanmakuViewPool : IPool<DanmakuView>
    {
        private const string TAG = "CachedDanmakuViewPool";
        /// <summary>
        /// 缓存DanmakuView队列。显示已经完毕的DanmakuView会被添加到缓存中进行复用。
        /// DanmakuView会被回收。
        /// </summary>
        internal LinkedList<DanmakuViewWithExpireTime> Cache = new LinkedList<DanmakuViewWithExpireTime>();
        /// <summary>
        /// 缓存存活时间
        /// </summary>
        internal long KeepAliveTime;
        /// <summary>
        /// 定时清理缓存
        /// </summary>
        private IScheduledExecutorService Checker = Executors.NewSingleThreadScheduledExecutor();
        /// <summary>
        /// 创建新DanmakuView的Creator
        /// </summary>
        private IViewCreator<DanmakuView> Creator;
        /// <summary>
        /// 最大DanmakuView数量。
        /// 这个数量包含了正在显示的DanmakuView和已经显示完毕进入缓存等待复用的DanmakuView之和。
        /// </summary>
        public int MaxSize { get; set; }
        /// <summary>
        /// 正在显示的弹幕数量
        /// </summary>
        internal int InUseSize;
        public CachedDanmakuViewPool(long keepAliveTime, int maxSize, IViewCreator<DanmakuView> creator)
        {
            KeepAliveTime = keepAliveTime;
            MaxSize = maxSize;
            Creator = creator;
            InUseSize = 0;
            ScheduleCheckUnusedViews();
        }
        private void ScheduleCheckUnusedViews()
        {
            Checker.ScheduleWithFixedDelay(new Runnable(() =>
            {
                MeVisual.Verbose(TAG, "scheduleCheckUnusedViews: mInUseSize=" + InUseSize + ", mCacheSize=" + Cache.Count);
                long current = JavaSystem.CurrentTimeMillis();
                while (Cache != null)
                {
                    DanmakuViewWithExpireTime first = Cache.FirstOrDefault();
                    if (current > first.ExpireTime)
                        Cache.Remove(first);
                    else
                        break;
                }
            }), 1000, 1000, TimeUnit.Milliseconds);
        }
        public int Count()
        {
            return Cache.Count + InUseSize;
        }
        public DanmakuView Get()
        {
            DanmakuView view;
            if (Cache == null)
            {
                // 缓存中没有View
                if (InUseSize >= MaxSize)
                    return null;
                view = Creator.Create();
            }
            else
            {
                // 有可用的缓存，从缓存中取
                view = Cache.Poll().DanmakuView;
            }
            view.AddOnExitListener(new DanmakuExitListener(this));
            InUseSize++;
            return view;
        }
        public void Release()
        {
            Cache.Clear();
        }
    }
}