using Android.OS;
using Java.Lang;
using System;

namespace Android.Content.PM
{
    public interface IPackageInstallerSession : IInterface
    {
        abstract class Stub : Binder, IPackageInstallerSession
        {
            public static IPackageInstallerSession AsInterface(IBinder binder)
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