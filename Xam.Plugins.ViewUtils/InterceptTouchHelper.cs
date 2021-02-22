using Android.Views;

namespace Xam.Plugins.ViewUtils
{
    public class InterceptTouchHelper
    {
        private ViewGroup Parent { get; }
        private GestureHelper GestureHelper { get; }
        public InterceptTouchHelper(ViewGroup parent, GestureHelper gestureHelper)
        {
            this.Parent = parent;
            this.GestureHelper = gestureHelper;
        }
        public InterceptTouchHelper(ViewGroup parent) : this(parent, GestureHelper.CreateDefault(parent.Context)) { }
        /// <summary>
        /// 判定是否拦截触摸事件
        /// </summary>
        /// <param name="event">触摸事件</param>
        /// <returns>true，拦截</returns>
        public bool OnInterceptTouchEvent(MotionEvent @event)
        {
            GestureHelper.OnTouchEvent(@event);
            return (GestureHelper.Gesture) switch
            {
                GestureHelper.GESTURE_LEFT => !CanChildrenScrollHorizontally(@event, 1) && CanParentScrollHorizontally(Parent, 1),
                GestureHelper.GESTURE_RIGHT => !CanChildrenScrollHorizontally(@event, -1) && CanParentScrollHorizontally(Parent, -1),
                GestureHelper.GESTURE_UP => !CanChildrenScrollVertically(@event, 1) && CanParentScrollVertically(Parent, 1),
                GestureHelper.GESTURE_DOWN => !CanChildrenScrollVertically(@event, -1) && CanParentScrollVertically(Parent, -1),
                _ => false,
            };
        }
        /// <summary>
        /// 判断子视图是否可以垂直滑动
        /// </summary>
        /// <param name="event">滑动事件</param>
        /// <param name="direction">方向：负数表示ScrollY值变小的方向；整数表示ScrollY值变大的方向</param>
        /// <returns>true，子View可以滑动</returns>
        protected bool CanChildrenScrollVertically(MotionEvent @event, int direction)
        {
            for (int i = 0; i < Parent.ChildCount; i++)
            {
                int index = Parent.ChildCount - 1 - i;
                View child = Parent.GetChildAt(index);
                if (child.Visibility == ViewStates.Visible && child.Enabled)
                {
                    float x = @event.GetX();
                    float y = @event.GetY();
                    if (x >= child.Left && x <= child.Right && y >= child.Top && y <= child.Bottom)
                    {
                        if (CanChildScrollVertically(child, direction))
                            return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 判断子View是否可以垂直滑动
        /// </summary>
        /// <param name="child">子View</param>
        /// <param name="direction">方向：负数表示ScrollY值变小的方向；整数表示ScrollY值变大的方向</param>
        /// <returns>true，可以滑动</returns>
        protected bool CanChildScrollVertically(View child, int direction)
        {
            return child.CanScrollVertically(direction);
        }
        /// <summary>
        /// 判断子视图是否可以水平滑动
        /// </summary>
        /// <param name="event">滑动事件</param>
        /// <param name="direction">方向：负数表示ScrollX值变小的方向；整数表示ScrollX值变大的方向</param>
        /// <returns>true，子View可以滑动</returns>
        protected bool CanChildrenScrollHorizontally(MotionEvent @event, int direction)
        {
            for (int i = 0; i < Parent.ChildCount; i++)
            {
                int index = Parent.ChildCount - 1 - i;
                View child = Parent.GetChildAt(index);
                if (child.Visibility == ViewStates.Visible && child.Enabled)
                {
                    float x = @event.GetX();
                    float y = @event.GetY();
                    if (x >= child.Left && x <= child.Right && y >= child.Top && y <= child.Bottom)
                    {
                        if (CanChildScrollHorizontally(child, direction))
                            return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 判断子View是否可以水平滑动
        /// </summary>
        /// <param name="child">子View</param>
        /// <param name="direction">方向：负数表示ScrollX值变小的方向；整数表示ScrollX值变大的方向</param>
        /// <returns>true，可以滑动</returns>
        protected bool CanChildScrollHorizontally(View child, int direction)
        {
            return child.CanScrollHorizontally(direction);
        }
        /// <summary>
        /// 判断父视图是否可以水平滑动
        /// </summary>
        /// <param name="parent">父视图</param>
        /// <param name="direction">方向</param>
        /// <returns>true，可以水平滑动</returns>
        protected bool CanParentScrollHorizontally(ViewGroup parent, int direction)
        {
            return parent.CanScrollHorizontally(direction);
        }
        /// <summary>
        /// 判断父视图是否可以垂直滑动
        /// </summary>
        /// <param name="parent">父视图</param>
        /// <param name="direction">方向</param>
        /// <returns>true，可以水平滑动</returns>
        protected bool CanParentScrollVertically(ViewGroup parent, int direction)
        {
            return parent.CanScrollVertically(direction);
        }
    }
}