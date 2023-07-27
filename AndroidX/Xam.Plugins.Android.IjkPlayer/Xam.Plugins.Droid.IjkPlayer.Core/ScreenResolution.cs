using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Java.Lang;
using Java.Lang.Reflect;
using System.Collections.Generic;

namespace Xam.Plugins.Droid.IjkPlayer.Core
{
    public class ScreenResolution
    {
        public static Dictionary<int, int> GetResolution(Context ctx)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
                return GetRealResolution(ctx);
            else
                return GetRealResolutionOnOldDevice(ctx);
        }
        private static Dictionary<int, int> GetRealResolutionOnOldDevice(Context ctx)
        {
            var dic = new Dictionary<int, int>();
            IWindowManager wm = ctx.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            try
            {
                Display display = wm.DefaultDisplay;
                var javaObj = (Object)display;
                var javaClass = javaObj.Class;
                Method mGetRawWidth = javaClass.GetDeclaredMethod("Width", new Class[] { Boolean.Type });
                Method mGetRawHeight = javaClass.GetDeclaredMethod("Height", new Class[] { Boolean.Type });
                mGetRawWidth.Accessible = true;
                mGetRawHeight.Accessible = true;
                int realWidth = (int)mGetRawWidth.Invoke(display, new Object[] { true });
                int realHeight = (int)mGetRawHeight.Invoke(display, new Object[] { true });
                dic.Add(realWidth, realHeight);
            }
            catch
            {
                DisplayMetrics dm = new DisplayMetrics();
                wm.DefaultDisplay.GetMetrics(dm);
                dic.Add(dm.WidthPixels, dm.HeightPixels);
            }
            return dic;
        }
        private static Dictionary<int, int> GetRealResolution(Context ctx)
        {
            IWindowManager wm = ctx.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            Display display = wm.DefaultDisplay;
            DisplayMetrics metrics = new DisplayMetrics();
            display.GetRealMetrics(metrics);
            return new Dictionary<int, int>
            {
                { metrics.WidthPixels, metrics.HeightPixels }
            };
        }
    }
}