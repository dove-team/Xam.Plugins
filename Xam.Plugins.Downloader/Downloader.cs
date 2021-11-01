using Android.App;
using Android.Content;
using System;
using System.Collections.Generic;

namespace Xam.Plugins.Downloader
{
    public sealed class Downloader
    {
        private Downloader() { }
        private static Downloader instance;
        public static Downloader Instance
        {
            get
            {
                if (instance == null)
                    instance = new Downloader();
                return instance;
            }
        }
        private bool init = false;
        public int NotificationID { get; internal set; }
        private ServiceConnection Connection { get; set; }
        public int NotifyIconResId { get; set; } = Resource.Drawable.notification_action_background;
        public void StartDownload(string url, Dictionary<string, string> headers)
        {
            if (!init)
                throw new ArgumentException("Init Downloader before call this.");
            if (Connection != null)
                Connection.DownloadBinder.StartDownload(url, headers);
        }
        public void PauseDownload()
        {
            Connection.DownloadBinder.PauseDownload();
        }
        public void CancelDownload()
        {
            Connection.DownloadBinder.CancelDownload();
        }
        public void Init(Activity activity, int notifyIconResId)
        {
            if (!init)
            {
                init = true;
                NotifyIconResId = notifyIconResId;
                Connection = new ServiceConnection();
                Intent intent = new Intent(activity, typeof(DownloadService));
                activity.StartService(intent);
                activity.BindService(intent, Connection, Bind.AutoCreate);
            }
        }
        public void Dispose(Activity activity)
        {
            if (init)
            {
                activity.UnbindService(Connection);
                init = false;
            }
        }
    }
}