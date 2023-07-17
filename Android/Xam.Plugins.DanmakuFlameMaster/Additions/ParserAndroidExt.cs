using Android.Runtime;
using System.IO;
using JavaObject = Java.Lang.Object;

namespace Master.Flame.Danmaku.Danmaku.Parser.Android
{
    public partial class AndroidFileSource : JavaObject, IDataSource
    {
        public unsafe Stream DataStream
        {
            get
            {
                const string __id = "data.()Ljava/io/InputStream;";
                try
                {
                    var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod(__id, this, null);
                    return InputStreamInvoker.FromJniHandle(__rm.Handle, JniHandleOwnership.TransferLocalRef);
                }
                finally { }
            }
        }
    }
}