using Android.Content;
using Android.Util;
using Android.Views;
using Java.Lang;

namespace Xam.Plugins.ViewUtils
{
    public class GestureHelper
    {
        /// <summary>
        /// 无手势，还不能确定手势
        /// </summary>
        public const int GESTURE_NONE = 0;
        /// <summary>
        /// 手势：按住
        /// </summary>
        public const int GESTURE_PRESSED = 1;
        /// <summary>
        /// 手势：点击
        /// </summary>
        public const int GESTURE_CLICK = 2;
        /// <summary>
        /// 手势：长按
        /// </summary>
        public const int GESTURE_LONG_CLICK = 3;
        /// <summary>
        /// 手势：左滑
        /// </summary>
        public const int GESTURE_LEFT = 4;
        /// <summary>
        /// 手势：上滑
        /// </summary>
        public const int GESTURE_UP = 5;
        /// <summary>
        /// 手势：右滑
        /// </summary>
        public const int GESTURE_RIGHT = 6;
        /// <summary>
        /// 手势：下滑
        /// </summary>
        public const int GESTURE_DOWN = 7;
        /// <summary>
        /// 默认的点大小，单位：dip
        /// </summary>
        public const float DEFAULT_FONT_SIZE_DP = 5;
        /// <summary>
        /// 默认的长按时间
        /// </summary>
        public const int DEFAULT_LONG_CLICK_TIME = 800;
        /// <summary>
        /// 点的大小
        /// </summary>
        private float pointSize;
        /// <summary>
        /// 长按判定时间
        /// </summary>
        private int longClickTime;
        private float xyScale;
        /// <summary>
        /// 手势
        /// </summary>
        public int Gesture { get; set; } = GESTURE_NONE;
        private long downTime;
        private float downX = 0f;
        private float downY = 0f;
        private float preX = 0f;
        private float preY = 0f;
        /// <summary>
        /// 创建一个手势帮助器
        /// </summary>
        /// <param name="pointSize">点的大小，超出此大小的滑动手势会被判定为非点击手势</param>
        /// <param name="longClickTime">长按点击时间，超过或等于此时间的按住手势算长按点击事件</param>
        /// <param name="xyScale">
        /// X轴与Y轴比例，影响方向手势的判定，默认是1；
        ///越小，手势判定越偏重于水平方向；
        ///越大，手势判定偏重于垂直方向；
        ///1，不偏重任何方向；
        ///如果是专注于水平方向，可以将此值设置小于1的数，
        ///如果是专注于垂直方向，可以将此值设置大于1的数；
        ///如果是垂直与水平同等重要，将此值设置成1
        ///</param>
        public GestureHelper(float pointSize, int longClickTime, float xyScale)
        {
            if (pointSize <= 0)
                throw new IllegalArgumentException("Illegal:pointSize <= 0");
            if (longClickTime <= 0)
                throw new IllegalArgumentException("Illegal:longClickTime <= 0");
            if (xyScale == 0)
                throw new IllegalArgumentException("Illegal:xyScale equals 0");
            this.pointSize = pointSize;
            this.longClickTime = longClickTime;
            this.xyScale = xyScale;
        }
        /// <summary>
        /// 创建默认的手势辅助器
        /// </summary>
        /// <param name="context">上下文对象</param>
        /// <returns>手势器</returns>
        public static GestureHelper CreateDefault(Context context)
        {
            float pointSize = TypedValue.ApplyDimension(ComplexUnitType.Dip, DEFAULT_FONT_SIZE_DP, context.Resources.DisplayMetrics);
            return new GestureHelper(pointSize, DEFAULT_LONG_CLICK_TIME, 1f);
        }
        /// <summary>
        /// 触发触摸滑动事件
        /// </summary>
        /// <param name="event">事件</param>
        public void OnTouchEvent(MotionEvent @event)
        {
            switch (@event.Action)
            {
                case MotionEventActions.Down:
                    TouchDown(@event);
                    break;
                case MotionEventActions.Move:
                    TouchMove(@event);
                    break;
                case MotionEventActions.Cancel:
                case MotionEventActions.Up:
                    TouchFinish();
                    break;
            }
        }
        /// <summary>
        /// 判定是否为水平滑动手势
        /// </summary>
        /// <returns>true，水平滑动手势</returns>
        public bool IsHorizontalGesture
        {
            get
            {
                return Gesture == GESTURE_LEFT || Gesture == GESTURE_RIGHT;
            }
        }
        /// <summary>
        /// 判定是否为垂直滑动手势
        /// </summary>
        /// <returns>true，垂直滑动手势</returns>
        public bool IsVerticalGesture
        {
            get
            {
                return Gesture == GESTURE_UP || Gesture == GESTURE_DOWN;
            }
        }
        private void TouchDown(MotionEvent @event)
        {
            downTime = JavaSystem.CurrentTimeMillis();
            downX = preX = @event.RawX;
            downY = preY = @event.RawY;
            Gesture = GESTURE_PRESSED;
        }
        private void TouchMove(MotionEvent @event)
        {
            float rangeX = @event.RawX - downX;
            float rangeY = @event.RawY - downY;
            if (Gesture == GESTURE_NONE || Gesture == GESTURE_PRESSED)
            {
                if (Math.Abs(rangeX) > pointSize || Math.Abs(rangeY) > pointSize)
                {
                    float ox = @event.RawX - preX;
                    float oy = @event.RawY - preY;
                    if (Math.Abs(ox) > xyScale * Math.Abs(oy))
                        Gesture = ox < 0 ? GESTURE_LEFT : GESTURE_RIGHT;
                    else
                        Gesture = oy < 0 ? GESTURE_UP : GESTURE_DOWN;
                }
                else
                    Gesture = GESTURE_PRESSED;
            }
            if (Gesture == GESTURE_PRESSED)
            {
                if (JavaSystem.CurrentTimeMillis() - downTime >= longClickTime)
                    Gesture = GESTURE_LONG_CLICK;
            }
            preX = @event.RawX;
            preY = @event.RawY;
        }
        private void TouchFinish()
        {
            if (Gesture == GESTURE_PRESSED)
            {
                if (JavaSystem.CurrentTimeMillis() - downTime >= longClickTime)
                    Gesture = GESTURE_LONG_CLICK;
                else
                    Gesture = GESTURE_CLICK;
            }
        }
    }
}