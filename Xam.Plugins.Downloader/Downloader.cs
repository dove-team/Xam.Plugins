using Android.App;
using Android.Content;
using System;

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
        private ServiceConnection Connection { get; set; }
        public DownloadBinder Binder
        {
            get
            {
                if (!init)
                    throw new ArgumentException("Init Downloader before call this.");
                return Connection?.DownloadBinder;
            }
        }
        public int NotifyIconResId { get; set; } = Resource.Drawable.notification_action_background;
        public void Init(Activity activity)
        {
            if (!init)
            {
                init = true;
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