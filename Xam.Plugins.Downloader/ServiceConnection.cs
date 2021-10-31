using Android.Content;
using Android.OS;
using Java.Lang;

namespace Xam.Plugins.Downloader
{
    internal sealed class ServiceConnection : Object, IServiceConnection
    {
        public DownloadBinder DownloadBinder { get; private set; }
        public void OnServiceConnected(ComponentName name, IBinder binder)
        {
            DownloadBinder = (DownloadBinder)binder;
        }
        public void OnServiceDisconnected(ComponentName name) { }
    }
}