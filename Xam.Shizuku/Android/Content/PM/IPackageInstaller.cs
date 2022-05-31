using Android.OS;
using System;

namespace Android.Content.PM
{
    public interface IPackageInstaller : IInterface
    {
        void AbandonSession(int sessionId) => throw new RemoteException();
        IPackageInstallerSession OpenSession(int sessionId)
             => throw new RemoteException();
        ParceledListSlice<PackageInstaller.SessionInfo> GetMySessions(string installerPackageName, int userId)
             => throw new RemoteException();
        abstract class Stub : Binder, IPackageInstaller
        {
            public static IPackageInstaller AsInterface(IBinder binder)
            {
                throw new NotImplementedException();
            }
            public IBinder AsBinder()
            {
                throw new NotImplementedException();
            }
        }
    }
}