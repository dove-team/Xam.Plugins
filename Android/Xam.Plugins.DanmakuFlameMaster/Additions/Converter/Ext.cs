using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Master.Flame.Danmaku.Additions
{
    public static class Ext
    {
        public static float ToSingle(this object token)
        {
            try
            {
                if (float.TryParse(token.ToString(), out float result))
                    return result;
                else
                    return Convert.ToSingle(token.ToString());
            }
            catch { return 0; }
        }
        public static int ToInt32(this object token)
        {
            try
            {
                if (int.TryParse(token.ToString(), out int result))
                    return result;
                else
                    return Convert.ToInt32(token.ToString());
            }
            catch { return -1; }
        }
        public static long ToInt64(this object obj)
        {
            try
            {
                if (long.TryParse(obj.ToString(), out long result))
                    return result;
                else
                    return Convert.ToInt64(obj.ToString());
            }
            catch { return default; }
        }
        public static uint ToUInt32(this object token)
        {
            try
            {
                if (uint.TryParse(token.ToString(), out uint result))
                    return result;
                else
                    return Convert.ToUInt32(token.ToString());
            }
            catch { return 0; }
        }
    }
}