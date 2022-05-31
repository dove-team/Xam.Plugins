using Android.OS;
using System;

namespace Android.Content
{
    public interface IIntentReceiver
    {
        void PerformReceive(Intent intent, int resultCode, String data, Bundle extras, bool ordered, bool sticky, int sendingUser)
          => throw new RemoteException();
        abstract class Stub : Binder, IIntentReceiver
        {

        }
    }
}