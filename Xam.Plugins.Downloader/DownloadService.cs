using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xam.Plugins.Downloader
{
    [Service]
    public sealed class DownloadService : Service
    {
        public string ChannelId { get; set; }
        internal string DownloadUrl;
        private DownloadBinder Binder;
        internal DownloadListener Listener;
        internal DownloadTask DownloadTask;
        public override IBinder OnBind(Intent intent)
        {
            return Binder;
        }
        public override void OnCreate()
        {
            Listener = new DownloadListener(this);
            Binder = new DownloadBinder(this);
        }
        internal NotificationManager NotificationManager
        {
            get => (NotificationManager)GetSystemService(NotificationService);
        }
        internal Notification GetNotification(string title, int progress)
        {
            Intent intent = new Intent(this, typeof(ActivationContext));
            PendingIntent pi = PendingIntent.GetActivity(this, 0, intent, 0);
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, ChannelId);
            builder.SetSmallIcon(Downloader.Instance.NotifyIconResId);
            builder.SetLargeIcon(BitmapFactory.DecodeResource(Resources, Downloader.Instance.NotifyIconResId));
            builder.SetContentIntent(pi);
            builder.SetContentTitle(title);
            if (progress > 0)
            {
                builder.SetContentText(progress + "%");
                builder.SetProgress(100, progress, false);
            }
            return builder.Build();
        }
    }
}