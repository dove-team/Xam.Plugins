using Android.OS;
using Java.Lang;
using System;

namespace Android.Content.PM
{
    public interface IPackageManager : IInterface
    {
        IPackageInstaller GetPackageInstaller() => throw new RemoteException();
        abstract class Stub : Binder, IPackageManager
        {
            public static IPackageManager AsInterface(IBinder obj)
            {
                throw new UnsupportedOperationException();
            }
            public IBinder AsBinder()
            {
                throw new NotImplementedException();
            }
        }
    }
}