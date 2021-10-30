using Android.Widget;

namespace Xam.Plugins.Downloader
{
    public sealed class DownloadListener : IDownloadListener
    {
        private DownloadService Service { get; }
        public DownloadListener(DownloadService service)
        {
            Service = service;
        }
        public void OnCanceled()
        {
            Service.DownloadTask = null;
            Service.StopForeground(true);
            Toast.MakeText(Service, "下载取消", ToastLength.Short).Show();
        }
        public void OnFailed()
        {
            Service.DownloadTask = null;
            Service.StopForeground(true);
            Service.NotificationManager.Notify(1, Service.GetNotification("下载失败", -1));
            Toast.MakeText(Service, "下载失败", ToastLength.Short).Show();
        }
        public void OnPaused()
        {
            Service.DownloadTask = null;
            Toast.MakeText(Service, "下载暂停", ToastLength.Short).Show();
        }
        public void OnProgress(int progress)
        {
            Service.NotificationManager.Notify(1, Service.GetNotification("Downloading...", progress));
        }
        public void OnSuccess()
        {
            Service.DownloadTask = null;
            Service.StopForeground(true);
            Service.NotificationManager.Notify(1, Service.GetNotification("下载中", -1));
            Toast.MakeText(Service, "下载成功", ToastLength.Short).Show();
        }
    }
}