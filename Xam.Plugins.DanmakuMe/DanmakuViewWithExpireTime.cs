namespace Xam.Plugins.DanmakuMe
{
    internal sealed class DanmakuViewWithExpireTime
    {
        /// <summary>
        /// 缓存的DanmakuView
        /// </summary>
        public DanmakuView DanmakuView { get; set; }
        /// <summary>
        /// 超过这个时间没有被访问的缓存将被丢弃
        /// </summary>
        public long ExpireTime { get; set; }
    }
}