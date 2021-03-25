using Android.Content;
using Android.Util;

namespace Xam.Plugins.DanmakuMe.Utils
{
    public sealed class ScreenUtil
    {
        /// <summary>
        /// 屏幕宽度
        /// </summary>
        public static int ScreenWidth { get; set; } = 1920;
        /// <summary>
        /// 屏幕高度
        /// </summary>
        private static int ScreenHeight { get; set; } = 1080;
        /// <summary>
        /// 设计宽度
        /// </summary>
        private static int DesignWidth = 1080;
        /// <summary>
        /// 设计高度
        /// </summary>
        private static int DesignHeight = 1920;
        /// <summary>
        /// 初始化ScreenUtil。在屏幕旋转之后，需要再次调用这个方法，否则计算将会出错。
        /// </summary>
        /// <param name="context"></param>
        public static void Init(Context context)
        {
            DisplayMetrics m = context.Resources.DisplayMetrics;
            ScreenWidth = m.WidthPixels;
            ScreenHeight = m.HeightPixels;
            if (DesignWidth > DesignHeight != ScreenWidth > ScreenHeight)
            {
                int tmp = DesignWidth;
                DesignWidth = DesignHeight;
                DesignHeight = tmp;
            }
        }
        public static void SetDesignWidthAndHeight(int width, int height)
        {
            DesignWidth = width;
            DesignHeight = height;
        }
        /// <summary>
        /// 根据实际屏幕和设计图的比例，自动缩放像素大小。
        /// 例如设计图大小是1920像素x1080像素，实际屏幕是2560像素x1440像素，那么对于一个设计图中100x100像素的方形，
        /// 实际屏幕中将会缩放为133像素x133像素。这有可能导致图形的失真（当实际的横竖比和设计图不同时）
        /// </summary>
        /// <param name="origin">设计图上的像素大小</param>
        /// <returns>实际屏幕中的尺寸</returns>
        public static int AutoSize(int origin)
        {
            return AutoWidth(origin);
        }
        /// <summary>
        /// 对于在横屏和竖屏下尺寸不同的物体，分别给出设计图的像素，返回实际屏幕中应有的像素。
        /// </summary>
        /// <param name="land">在横屏设计图中的像素大小</param>
        /// <param name="port">在竖屏设计图中的像素大小</param>
        /// <returns></returns>
        public static int AutoSize(int land, int port)
        {
            return IsPortrait ? AutoSize(port) : AutoSize(land);
        }
        /// <summary>
        /// 根据屏幕分辨率自适应宽度
        /// </summary>
        /// <param name="origin">设计图中的宽度，像素</param>
        /// <returns>实际屏幕中的宽度，像素</returns>
        public static int AutoWidth(int origin)
        {
            if (ScreenWidth == 0 || DesignWidth == 0)
                return origin;
            int autoSize = origin * ScreenWidth / DesignWidth;
            if (origin != 0 && autoSize == 0)
                return 1;
            return autoSize;
        }
        /// <summary>
        /// 根据屏幕分辨率自适应高度
        /// </summary>
        /// <param name="origin">设计图中的高度，像素</param>
        /// <returns>实际屏幕中的高度，像素</returns>
        public static int AutoHeight(int origin)
        {
            if (ScreenHeight == 0 || DesignHeight == 0)
                return origin;
            int auto = origin * ScreenHeight / DesignHeight;
            if (origin != 0 && auto == 0)
                return 1;
            return auto;
        }
        /// <summary>
        /// 是否是竖屏
        /// </summary>
        public static bool IsPortrait
        {
            get
            {
                return ScreenHeight > ScreenWidth;
            }
        }
    }
}