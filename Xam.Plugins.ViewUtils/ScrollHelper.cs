using Android.Content;
using Android.Views;

namespace Xam.Plugins.ViewUtils
{
    public abstract class ScrollHelper
    {
        private float startTouchX;
        private float startTouchY;
        private int startScrollX;
        private int startScrollY;
        private GestureHelper GestureHelper { get; }
        private VelocityTracker VelocityTracker { get; }
        public ScrollHelper(GestureHelper gestureHelper)
        {
            this.GestureHelper = gestureHelper;
            this.VelocityTracker = VelocityTracker.Obtain();
        }
        public ScrollHelper(Context context) : this(GestureHelper.CreateDefault(context)) { }
        /// <summary>
        /// 触发触摸事件
        /// </summary>
        /// <param name="event">事件</param>
        public void OnTouchEvent(MotionEvent @event)
        {
            GestureHelper.OnTouchEvent(@event);
            VelocityTracker.AddMovement(@event);
            switch (@event.Action)
            {
                case MotionEventActions.Down:
                    {
                        SetStartPosition(@event.GetX(), @event.GetY());
                        break;
                    }
                case MotionEventActions.Move:
                    {
                        if (CanScroll)
                        {
                            float rangeX = @event.GetX() - startTouchX;
                            float rangeY = @event.GetY() - startTouchY;
                            int dstX = (int)(startScrollX - rangeX);
                            int dstY = (int)(startScrollY - rangeY);
                            if (dstX < GetMinHorizontallyScroll)
                            {
                                dstX = 0;
                                startTouchX = @event.GetX();
                                startScrollX = dstX;
                            }
                            else if (dstX > GetMaxHorizontallyScroll)
                            {
                                dstX = GetViewHorizontallyScrollSize();
                                startTouchX = @event.GetX();
                                startScrollX = dstX;
                            }
                            if (dstY < GetMinVerticallyScroll)
                            {
                                dstY = 0;
                                startTouchY = @event.GetY();
                                startScrollY = dstY;
                            }
                            else if (dstY > GetMaxVerticallyScroll)
                            {
                                dstY = GetViewVerticallyScrollSize();
                                startTouchY = @event.GetY();
                                startScrollY = dstY;
                            }
                            ViewScrollTo(dstX, dstY);
                        }
                        break;
                    }
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    {
                        VelocityTracker.ComputeCurrentVelocity(1000);
                        if (CanScroll)
                        {
                            float xv = VelocityTracker.XVelocity;
                            float yv = VelocityTracker.YVelocity;
                            ViewFling(xv, yv);
                        }
                        break;
                    }
            }
        }
        /// <summary>
        /// 设置起始位置，一般是ACTION_DOWN的时候执行，如果有特殊要求，可以在外部主动调用，更改起始位置
        /// </summary>
        /// <param name="x">位置X</param>
        /// <param name="y">位置Y</param>
        public void SetStartPosition(float x, float y)
        {
            startTouchX = x;
            startTouchY = y;
            startScrollX = GetViewScrollX();
            startScrollY = GetViewScrollY();
        }
        /// <summary>
        /// 判断是否可以滑动
        /// </summary>
        /// <returns>是否可以滑动</returns>
        protected bool CanScroll
        {
            get
            {
                return GestureHelper.IsVerticalGesture || GestureHelper.IsHorizontalGesture;
            }
        }
        /// <summary>
        /// 获取水平方向最小的滑动位置
        /// </summary>
        /// <returns>水平方向最小的滑动位置</returns>
        public int GetMinHorizontallyScroll
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取水平方向最大的滑动位置
        /// </summary>
        /// <returns>水平方向最大的滑动位置</returns>
        public int GetMaxHorizontallyScroll
        {
            get
            {
                return GetViewHorizontallyScrollSize();
            }
        }
        /// <summary>
        /// 获取垂直方向最小的滑动位置
        /// </summary>
        /// <returns>垂直方向最小的滑动位置</returns>
        public int GetMinVerticallyScroll
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取垂直方向最大的滑动位置
        /// </summary>
        /// <returns>垂直方向最大的滑动位置</returns>
        public int GetMaxVerticallyScroll
        {
            get
            {
                return GetViewVerticallyScrollSize();
            }
        }
        /// <summary>
        /// 获取视图滑动位置X
        /// </summary>
        /// <returns>视图滑动位置Y</returns>
        protected abstract int GetViewScrollX();
        /// <summary>
        /// 获取视图滑动位置Y
        /// </summary>
        /// <returns>视图滑动位置Y</returns>
        protected abstract int GetViewScrollY();
        /// <summary>
        /// 获取视图水平方向可以滑动的范围，一般在此方法返回
        /// {@link ViewGroup#computeHorizontalScrollRange() ViewGroup.computeHorizontalScrollRange} 减去
        /// {@link ViewGroup#computeHorizontalScrollExtent() ViewGroup.computeHorizontalScrollExtent} 的差
        /// result = range-extent
        /// </summary>
        /// <returns>水平方向可以滑动的范围</returns>
        protected abstract int GetViewHorizontallyScrollSize();
        /// <summary>
        /// 获取视图垂直方向可以滑动的范围，一般在此方法返回
        /// {@link ViewGroup#computeVerticalScrollRange() ViewGroup.computeVerticalScrollRange} 减去
        /// {@link ViewGroup#computeVerticalScrollExtent() ViewGroup.computeVerticalScrollExtent} 的差
        /// result = range-extent
        /// </summary>
        /// <returns>垂直方向可以滑动的范围</returns>
        protected abstract int GetViewVerticallyScrollSize();
        /// <summary>
        /// 将视图滑动至指定位置，一般调用{@link android.view.View#scrollTo(int, int) View.scrollTo}方法即可
        /// </summary>
        /// <param name="x">位置X</param>
        /// <param name="y">位置Y</param>
        protected abstract void ViewScrollTo(int x, int y);
        /// <summary>
        /// 当触摸抬起时，执行此方法，一般在此方法内执行
        /// {@link android.widget.Scroller#fling(int, int, int, int, int, int, int, int) Scroller.fling}
        /// 方法，需要注意的是，速度应该取参数的相反值，因为参数的速度表示的是触摸滑动的速度，刚好与滑动的速度方向相反。
        /// </summary>
        /// <param name="xv">水平触摸滑动的速度</param>
        /// <param name="yv">垂直触摸滑动的速度</param>
        protected abstract void ViewFling(float xv, float yv);
    }
}