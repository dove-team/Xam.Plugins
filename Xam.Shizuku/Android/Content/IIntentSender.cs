using Android.OS;
using AndroidX.Annotations;
using Java.Lang;

namespace Android.Content
{
    public interface IIntentSender : IInterface
    {
        int Send(int code, Intent intent, string resolvedType, IIntentReceiver finishedReceiver, string requiredPermission, Bundle options);
        [RequiresApi(26)]
        void send(int code, Intent intent, string resolvedType, IBinder whitelistToken, IIntentReceiver finishedReceiver, string requiredPermission, Bundle options);
        abstract class Stub : Binder, IIntentSender
        {
            public Stub()
            {
                throw new UnsupportedOperationException();
            }
            public IBinder AsBinder()
            {
                throw new UnsupportedOperationException();
            }
            public static IIntentSender AsInterface(IBinder binder)
            {
                throw new UnsupportedOperationException();
            }
            public int Send(int code, Intent intent, string resolvedType, IIntentReceiver finishedReceiver, string requiredPermission, Bundle options)
            {
                throw new UnsupportedOperationException();
            }
            public void send(int code, Intent intent, string resolvedType, IBinder whitelistToken, IIntentReceiver finishedReceiver, string requiredPermission, Bundle options)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}