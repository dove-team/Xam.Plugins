using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using System;

namespace Xam.Plugins.Downloader
{
    [Service]
    public sealed class DownloadService : Service
    {
        internal string DownloadUrl;
        private DownloadBinder Binder;
        internal DownloadListener Listener;
        internal DownloadTask DownloadTask;
        private readonly string CHANNEL_ID = Guid.NewGuid().ToString("N");
        private const string CHANNEL_NAME = "Xam.Plugins.Downloader";
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
            Intent intent = new Intent(this, typeof(Activity));
            PendingIntent pi = PendingIntent.GetActivity(this, 0, intent, 0);
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, CHANNEL_ID);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                NotificationChannel channel = new NotificationChannel(CHANNEL_ID, CHANNEL_NAME, NotificationImportance.Default);
                channel.EnableVibration(false);
                notificationManager.CreateNotificationChannel(channel);
            }
            builder.SetSmallIcon(Downloader.Instance.NotifyIconResId);
            builder.SetLargeIcon(BitmapFactory.DecodeResource(Resources, Downloader.Instance.NotifyIconResId));
            builder.SetContentIntent(pi);
            builder.SetContentTitle(title);
            if (progress > 0)
            {
                builder.SetContentText(progress + "%");
                builder.SetProgress(100, progress, false);
            }
            StartForeground(Downloader.Instance.NotificationID, builder.Build());
            return builder.Build();
        }
    }
}