namespace Xam.Plugins.Downloader
{
    public interface IDownloadListener
    {
        /// <summary>
        /// 用于通知当前下载进度
        /// </summary>
        /// <param name="progress"></param>
        void OnProgress(int progress);
        /// <summary>
        /// 用于通知下载成功
        /// </summary>
        void OnSuccess();
        /// <summary>
        /// 用于通知下载失败
        /// </summary>
        void OnFailed();
        /// <summary>
        /// 用于通知下载暂停
        /// </summary>
        void OnPaused();
        /// <summary>
        /// 用于通知下载取消
        /// </summary>
        void OnCanceled();
    }
}