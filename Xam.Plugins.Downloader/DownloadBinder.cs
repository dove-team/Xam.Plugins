using Android.OS;
using Android.Runtime;
using Android.Widget;
using Java.IO;
using System;
using System.Collections.Generic;
using Environment = Android.OS.Environment;

namespace Xam.Plugins.Downloader
{
    internal sealed class DownloadBinder : Binder
    {
        private DownloadService Service { get; }
        public DownloadBinder(DownloadService service)
        {
            Service = service;
        }
        public DownloadBinder(IntPtr reference, JniHandleOwnership transfer) : base(reference, transfer) { }
        public void StartDownload(string url, Dictionary<string, string> headers)
        {
            try
            {
                if (Service.DownloadTask == null)
                {
                    Service.DownloadUrl = url;
                    Service.DownloadTask = new DownloadTask(Service.Listener);
                    Service.DownloadTask.Execute(Service.DownloadUrl, headers);
                    Service.StartForeground(Downloader.Instance.NotificationID, Service.GetNotification("Downloading...", 0));
                    Toast.MakeText(Service, "下载中", ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw ex;
            }
        }
        public void PauseDownload()
        {
            Service.DownloadTask?.PauseDownload();
        }
        public void CancelDownload()
        {
            if (Service.DownloadTask != null)
            {
                Service.DownloadTask.CancelDownload();
            }
            else
            {
                if (Service.DownloadUrl != null)
                {
                    var fileName = Service.DownloadUrl[Service.DownloadUrl.LastIndexOf("/")..];
                    var directory = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads).Path;
                    File file = new File(directory + fileName);
                    if (file.Exists())
                        file.Delete();
                    Service.NotificationManager.Cancel(1);
                    Service.StopForeground(true);
                    Toast.MakeText(Service, "取消", ToastLength.Short).Show();
                }
            }
        }
    }
}