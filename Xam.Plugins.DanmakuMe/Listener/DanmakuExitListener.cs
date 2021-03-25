using Java.Lang;
using Xam.Plugins.DanmakuMe.Interface;

namespace Xam.Plugins.DanmakuMe.Listener
{
    public class DanmakuExitListener : Object, IOnExitListener
    {
        private CachedDanmakuViewPool DanmakuViewPool { get; }
        public DanmakuExitListener(CachedDanmakuViewPool pool)
        {
            this.DanmakuViewPool = pool;
        }
        public void OnExit(DanmakuView view)
        {
            long expire = JavaSystem.CurrentTimeMillis() + DanmakuViewPool.KeepAliveTime;
            view.Restore();
            DanmakuViewWithExpireTime item = new DanmakuViewWithExpireTime
            {
                DanmakuView = view,
                ExpireTime = expire
            };
            DanmakuViewPool.Cache.Offer(item);
            DanmakuViewPool.InUseSize--;
        }
    }
}