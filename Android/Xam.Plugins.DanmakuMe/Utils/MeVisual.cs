using Android.Util;
using System.Text;

namespace Xam.Plugins.DanmakuMe.Utils
{
    public class MeVisual
    {
        private static bool Enabled { get; set; } = false;
        public static void Verbose(string tag, string msg)
        {
            if (Enabled)
                Log.Verbose(tag, msg);
        }
        public static void Debug(string tag, string msg)
        {
            if (Enabled)
                Log.Debug(tag, msg);
        }
        public static void Info(string tag, string msg)
        {
            if (Enabled)
                Log.Info(tag, msg);
        }
        public static void Warn(string tag, string msg)
        {
            if (Enabled)
                Log.Warn(tag, msg);
        }
        public static void Error(string tag, string msg)
        {
            if (Enabled)
                Log.Error(tag, msg);
        }
        public static void Debug(string tag, object[] args)
        {
            if (Enabled)
            {
                StringBuilder msg = new StringBuilder();
                foreach (object arg in args)
                    msg.Append(arg).Append(" ");
                Log.Debug(tag, msg.ToString());
            }
        }
    }
}