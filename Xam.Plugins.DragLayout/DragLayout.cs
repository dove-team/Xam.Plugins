using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System.Collections.Generic;
using Xam.Plugins.DragLayout.Interfaces;
using Xam.Plugins.DragLayout.Listeners;
using Xam.Plugins.ViewUtils;

namespace Xam.Plugins.DragLayout
{
    public class DragLayout : ViewGroup
    {
        /// <summary>
        /// 默认滑动速度，dip/秒
        /// </summary>
        public const int DEFAULT_SCROLL_VELOCITY_DP = 300;
        /// <summary>
        /// 不可以超出滑动范围
        /// </summary>
        public const int OVER_SCROLL_NONE = 0x00;
        /// <summary>
        /// 左边可以超出滑动范围
        /// </summary>
        public const int OVER_SCROLL_LEFT = 0x01;
        /// <summary>
        /// 上边可以超出滑动范围
        /// </summary>
        public const int OVER_SCROLL_TOP = 0x02;
        /// <summary>
        /// 右边可以超出滑动范围
        /// </summary>
        public const int OVER_SCROLL_RIGHT = 0x04;
        /// <summary>
        /// 下边可以超出滑动范围
        /// </summary>
        public const int OVER_SCROLL_BOTTOM = 0x08;
        /// <summary>
        /// 所有方向都可以超出滑动范围
        /// </summary>
        public const int OVER_SCROLL_ALL = OVER_SCROLL_LEFT | OVER_SCROLL_TOP | OVER_SCROLL_RIGHT | OVER_SCROLL_BOTTOM;
        /// <summary>
        /// 边缘大小
        /// </summary>
        private Rect EdgeSize { get; } = new Rect();
        /// <summary>
        /// 滑动层滑动位置X
        /// </summary>
        public int LayerScrollX { get; set; }
        /// <summary>
        /// 滑动层滑动位置Y
        /// </summary>
        public int LayerScrollY { get; set; }
        /// <summary>
        /// 中间的区间
        /// </summary>
        private Rect CenterRect { get; } = new Rect();
        /// <summary>
        /// 是否可以超出滑动范围
        /// </summary>
        private int overScroll = OVER_SCROLL_ALL;
        /// <summary>
        /// 滑动速度
        /// </summary>
        private int scrollVelocity;
        /// <summary>
        /// 最大滑动时间，大于0，表示smoothLayer操作有最大时间限制，如果按照scrollVelocity的速度没能在最大
        /// 时间内完成，则smoothLayer操作不会按照scrollVelocity，其速度会被重新计算成刚好在最大时间内完成
        /// 的速度，此速度默认为0，即关闭最大时间限制
        /// </summary>
        private int maxScrollTime;
        /// <summary>
        /// 是否开启触摸滑动，默认开启
        /// </summary>
        private bool touchScrollable;
        public LinkedList<Runnable> afterLayoutRunnableList;
        /// <summary>
        /// 是否添加到窗口系统
        /// </summary>
        private bool attached = false;
        /// <summary>
        /// 辅助Gravity计算Rect
        /// </summary>
        private Rect OutRect { get; } = new Rect();
        /// <summary>
        /// 辅助Gravity计算Rect
        /// </summary>
        private Rect ContainerRect { get; } = new Rect();
        /// <summary>
        /// 触摸滑动辅助工具
        /// </summary>
        private GestureHelper gestureHelper;
        /// <summary>
        /// 速度辅助工具
        /// </summary>
        private VelocityTracker velocityTracker;
        /// <summary>
        /// 开始触摸时滑动层的滑动位置X
        /// </summary>
        private int touchScrollStartX;
        /// <summary>
        /// 开始触摸时滑动层的滑动位置Y
        /// </summary>
        private int touchScrollStartY;
        /// <summary>
        /// 开始触摸滑动位置：X
        /// </summary>
        private float touchStartX;
        /// <summary>
        /// 开始触摸滑动位置：Y
        /// </summary>
        private float touchStartY;
        /// <summary>
        /// 是否正在滑动中
        /// </summary>
        private bool touching;
        /// <summary>
        /// 滑动层的滑动器
        /// </summary>
        private Scroller layerScroller;
        /// <summary>
        /// 视图信息hash code，如果视图发生变化，此hashCode会发生改变
        /// </summary>
        private long viewInfoCode = 0;
        public IOnLayerScrollChangedListener OnLayerScrollChangedListener { get; set; }
        public ICanOpenEdgeCallback CanOpenEdgeCallback { get; set; }
        public ICanChildScrollCallback CanChildScrollCallback { get; set; }
        private Runnable updateLayerScrollRunnable;
        public DragLayout(Context context) : base(context)
        {
            Init(null);
        }
        public DragLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(attrs);
        }
        public DragLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init(attrs);
        }
        public DragLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init(attrs);
        }
        private void Init(IAttributeSet attrs)
        {
            scrollVelocity = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip,
                    DEFAULT_SCROLL_VELOCITY_DP, Context.Resources.DisplayMetrics);
            touchScrollable = true;
            if (null != attrs)
            {
                TypedArray ta = Context.ObtainStyledAttributes(attrs, Resource.Styleable.DragLayout);
                overScroll = ta.GetInt(Resource.Styleable.DragLayout_overScroll, OVER_SCROLL_ALL);
                scrollVelocity = ta.GetDimensionPixelSize(
                        Resource.Styleable.DragLayout_scrollVelocity, scrollVelocity);
                maxScrollTime = ta.GetInt(Resource.Styleable.DragLayout_maxScrollTime, 0);
                touchScrollable = ta.GetBoolean(Resource.Styleable.DragLayout_touchScrollable, touchScrollable);
                ta.Recycle();
            }
            gestureHelper = GestureHelper.CreateDefault(Context);
            layerScroller = new Scroller(Context);
            velocityTracker = VelocityTracker.Obtain();
            ViewTreeObserver.AddOnGlobalLayoutListener(new DragLayoutListener(this));
        }
        protected override LayoutParams GenerateLayoutParams(LayoutParams lp)
        {
            return new LayoutParams(lp);
        }
        protected override LayoutParams GenerateDefaultLayoutParams()
        {
            return new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
        }
        public new LayoutParams GenerateLayoutParams(IAttributeSet attrs)
        {
            return new LayoutParams(Context, attrs);
        }
        public override bool OnInterceptTouchEvent(MotionEvent @event)
        {
            HandleTouchEvent(@event);
            switch (gestureHelper.Gesture)
            {
                case GestureHelper.GESTURE_LEFT:
                    if (LayerScrollX != 0)
                        return true;
                    return !CanChildrenScrollHorizontally(@event, 1) && CanLayerScrollHorizontally(1);
                case GestureHelper.GESTURE_RIGHT:
                    if (LayerScrollX != 0)
                        return true;
                    return !CanChildrenScrollHorizontally(@event, -1) &&
                            CanLayerScrollHorizontally(-1);
                case GestureHelper.GESTURE_UP:
                    if (LayerScrollY != 0)
                        return true;
                    return !CanChildrenScrollVertically(@event, 1) &&
                            CanLayerScrollVertically(1);
                case GestureHelper.GESTURE_DOWN:
                    if (LayerScrollY != 0)
                        return true;
                    return !CanChildrenScrollVertically(@event, -1) &&
                            CanLayerScrollVertically(-1);
                default:
                    if (@event.Action == MotionEventActions.Up || @event.Action == MotionEventActions.Cancel)
                        ReleaseTouch();
                    return false;
            }
        }
        public override bool PerformClick()
        {
            return base.PerformClick();
        }
        public override bool OnTouchEvent(MotionEvent @event)
        {
            if (@event.Action == MotionEventActions.Up)
                PerformClick();
            HandleTouchEvent(@event);
            var resetTouch = false;
            var vertical = false;
            var horizontal = false;
            switch (gestureHelper.Gesture)
            {
                case GestureHelper.GESTURE_RIGHT:
                case GestureHelper.GESTURE_LEFT:
                    if (LayerScrollY != 0)
                        vertical = true;
                    else
                        horizontal = true;
                    break;
                case GestureHelper.GESTURE_DOWN:
                case GestureHelper.GESTURE_UP:
                    if (LayerScrollX != 0)
                        horizontal = true;
                    else
                        vertical = true;
                    break;
                default:
                    if (@event.Action == MotionEventActions.Up || @event.Action == MotionEventActions.Cancel)
                        ReleaseTouch();
                    break;
            }
            if (vertical)
            {
                float rangeY = @event.GetY() - touchStartY;
                int dy = (int)(touchScrollStartY - rangeY);
                if (!CanVerticalScrollTo(dy))
                {
                    resetTouch = true;
                    if (dy < 0)
                        dy = VerticalLayerScrollMin;
                    else if (dy > 0)
                        dy = VerticalLayerScrollMax;
                }
                if (touchScrollable)
                {
                    LayerScrollTo(LayerScrollX, dy);
                }
                if (@event.Action == MotionEventActions.Up || @event.Action == MotionEventActions.Cancel)
                {
                    velocityTracker.ComputeCurrentVelocity(1000);
                    if (LayerScrollY < 0)
                    {
                        if (velocityTracker.YVelocity > 0)
                        {
                            if (CanOpenTop(velocityTracker.XVelocity, velocityTracker.YVelocity))
                                SmoothLayerScrollTo(LayerScrollX, VerticalLayerScrollMin);
                            else
                                SmoothLayerScrollTo(0, 0);
                        }
                        else
                        {
                            SmoothLayerScrollTo(0, 0);
                        }
                    }
                    else if (LayerScrollY > 0)
                    {
                        if (velocityTracker.YVelocity < 0)
                        {
                            if (CanOpenBottom(velocityTracker.XVelocity, velocityTracker.YVelocity))
                                SmoothLayerScrollTo(LayerScrollX, VerticalLayerScrollMax);
                            else
                                SmoothLayerScrollTo(0, 0);
                        }
                        else
                        {
                            SmoothLayerScrollTo(0, 0);
                        }
                    }
                }
            }
            else if (horizontal)
            {
                float rangeX = @event.GetX() - touchStartX;
                int dx = (int)(touchScrollStartX - rangeX);
                if (!CanHorizontalScrollTo(dx))
                {
                    resetTouch = true;
                    if (dx < 0)
                        dx = HorizontalLayerScrollMin;
                    else if (dx > 0)
                        dx = HorizontalLayerScrollMax;
                }
                if (touchScrollable)
                {
                    LayerScrollTo(dx, ScrollY);
                }
                if (@event.Action == MotionEventActions.Up ||
                        @event.Action == MotionEventActions.Cancel)
                {
                    velocityTracker.ComputeCurrentVelocity(1000);
                    if (LayerScrollX < 0)
                    {
                        if (velocityTracker.XVelocity > 0)
                        {
                            if (CanOpenLeft(velocityTracker.XVelocity, velocityTracker.YVelocity))
                            {
                                SmoothLayerScrollTo(HorizontalLayerScrollMin, LayerScrollY);
                            }
                            else
                            {
                                SmoothLayerScrollTo(0, 0);
                            }
                        }
                        else
                        {
                            SmoothLayerScrollTo(0, 0);
                        }
                    }
                    else if (LayerScrollX > 0)
                    {
                        if (velocityTracker.XVelocity < 0)
                        {
                            if (CanOpenRight(velocityTracker.XVelocity, velocityTracker.YVelocity))
                                SmoothLayerScrollTo(HorizontalLayerScrollMax, LayerScrollY);
                            else
                                SmoothLayerScrollTo(0, 0);
                        }
                        else
                            SmoothLayerScrollTo(0, 0);
                    }
                }
            }
            if (resetTouch)
                ResetTouchStart(@event.GetX(), @event.GetY());
            return true;
        }
        private void HandleTouchEvent(MotionEvent @event)
        {
            velocityTracker.AddMovement(@event);
            gestureHelper.OnTouchEvent(@event);
            if (@event.Action == MotionEventActions.Down)
            {
                touching = true;
                ResetTouchStart(@event.GetX(), @event.GetY());
                if (!layerScroller.IsFinished)
                    layerScroller.AbortAnimation();
            }
            else if (@event.Action == MotionEventActions.Up || @event.Action == MotionEventActions.Cancel)
                touching = false;
        }
        private void ResetTouchStart(float x, float y)
        {
            touchScrollStartX = LayerScrollX;
            touchScrollStartY = LayerScrollY;
            touchStartX = x;
            touchStartY = y;
        }
        protected virtual bool CanOpenLeft(float xv, float yv)
        {
            return (null != CanOpenEdgeCallback && CanOpenEdgeCallback.CanOpenLeft(this, xv, yv)) || LayerScrollX <= HorizontalLayerScrollMin;
        }
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            long code = ComputeInfoCode((long)widthMeasureSpec + heightMeasureSpec);
            if (code == viewInfoCode)
            {
                SetMeasuredDimension(MeasuredWidth, MeasuredHeight);
                return;
            }
            viewInfoCode = code;
            int paddingWidth = PaddingLeft + PaddingRight;
            int paddingHeight = PaddingTop + PaddingBottom;
            int smsw = MeasureUtils.MakeSelfMeasureSpec(widthMeasureSpec, paddingWidth);
            int smsh = MeasureUtils.MakeSelfMeasureSpec(heightMeasureSpec, paddingHeight);
            int smswnp = MeasureUtils.MakeSelfMeasureSpec(widthMeasureSpec, 0);
            int smshnp = MeasureUtils.MakeSelfMeasureSpec(heightMeasureSpec, 0);
            int contentWidth = 0;
            int contentHeight = 0;
            for (int i = 0; i < ChildCount; i++)
            {
                View child = GetChildAt(i);
                if (child.Visibility == ViewStates.Gone) continue;
                ViewLayoutParams lp = (ViewLayoutParams)child.LayoutParameters;
                int marginWidth = lp.LeftMargin + lp.RightMargin;
                int marginHeight = lp.TopMargin + lp.BottomMargin;
                int cmsw, cmsh;
                switch (lp.Layer)
                {
                    case ViewLayoutParams.LAYER_LEFT:
                    case ViewLayoutParams.LAYER_RIGHT:
                    case ViewLayoutParams.LAYER_TOP:
                    case ViewLayoutParams.LAYER_BOTTOM:
                        cmsw = MeasureUtils.MakeChildMeasureSpec(smswnp, lp.Width, marginWidth);
                        cmsh = MeasureUtils.MakeChildMeasureSpec(smshnp, lp.Height, marginHeight);
                        break;
                    default:
                        cmsw = MeasureUtils.MakeChildMeasureSpec(smsw, lp.Width, marginWidth);
                        cmsh = MeasureUtils.MakeChildMeasureSpec(smsh, lp.Height, marginHeight);
                        break;
                }
                child.Measure(cmsw, cmsh);
                int cw = marginWidth + child.MeasuredWidth;
                int ch = marginHeight + child.MeasuredHeight;
                if (cw > contentWidth)
                    contentWidth = cw;
                if (ch > contentHeight)
                    contentHeight = ch;
            }
            contentWidth += paddingWidth;
            contentHeight += paddingHeight;
            int width = MeasureUtils.GetMeasuredDimension(contentWidth, widthMeasureSpec);
            int height = MeasureUtils.GetMeasuredDimension(contentHeight, heightMeasureSpec);
            SetMeasuredDimension(width, height);
            EdgeSize.SetEmpty();
            CenterRect.Left = PaddingLeft;
            CenterRect.Top = PaddingTop;
            int centerWidth = Math.Max(MeasuredWidth - PaddingLeft - PaddingRight, 0);
            int centerHeight = Math.Max(MeasuredHeight - PaddingTop - PaddingBottom, 0);
            CenterRect.Right = CenterRect.Left + centerWidth;
            CenterRect.Bottom = CenterRect.Top + centerHeight;
            for (int i = 0; i < ChildCount; i++)
            {
                View child = GetChildAt(i);
                if (child.Visibility == ViewStates.Gone) continue;
                ViewLayoutParams lp = (ViewLayoutParams)child.LayoutParameters;
                int widthSpace = child.MeasuredWidth + lp.LeftMargin + lp.RightMargin;
                int heightSpace = child.MeasuredHeight + lp.TopMargin + lp.BottomMargin;
                switch (lp.Layer)
                {
                    case ViewLayoutParams.LAYER_LEFT:
                        if (widthSpace > EdgeSize.Left)
                            EdgeSize.Left = widthSpace;
                        break;
                    case ViewLayoutParams.LAYER_TOP:
                        if (heightSpace > EdgeSize.Top)
                            EdgeSize.Top = heightSpace;
                        break;
                    case ViewLayoutParams.LAYER_RIGHT:
                        if (widthSpace > EdgeSize.Right)
                            EdgeSize.Right = widthSpace;
                        break;
                    case ViewLayoutParams.LAYER_BOTTOM:
                        if (heightSpace > EdgeSize.Bottom)
                            EdgeSize.Bottom = heightSpace;
                        break;
                    case ViewLayoutParams.LAYER_NONE:
                    case ViewLayoutParams.LAYER_CENTER:
                    default:
                        break;
                }
            }
        }
        public void SmoothLayerScrollTo(int x, int y)
        {
            if (touching) return;
            int dx = x - LayerScrollX;
            int dy = y - LayerScrollY;
            if (!layerScroller.IsFinished)
                layerScroller.AbortAnimation();
            float size = Math.Max(Math.Abs(dx), Math.Abs(dy));
            int duration;
            if (scrollVelocity > 0)
                duration = (int)(size / scrollVelocity * 1000);
            else
                duration = 100;
            if (maxScrollTime > 0)
            {
                if (duration > maxScrollTime)
                    duration = maxScrollTime;
            }
            layerScroller.StartScroll(LayerScrollX, LayerScrollY, dx, dy, duration);
            UpdateLayerScroll();
        }
        public void UpdateLayerScroll()
        {
            RemoveUpdateLayerScrollRunnable();
            updateLayerScrollRunnable = new Runnable(() =>
            {
                updateLayerScrollRunnable = null;
                ComputeLayerScroll();
            });
            PostDelayed(updateLayerScrollRunnable, 10);
        }
        protected void ComputeLayerScroll()
        {
            if (layerScroller.ComputeScrollOffset())
            {
                int x = layerScroller.CurrX;
                int y = layerScroller.CurrY;
                LayerScrollTo(x, y);
                UpdateLayerScroll();
            }
        }
        private void RemoveUpdateLayerScrollRunnable()
        {
            if (null != updateLayerScrollRunnable)
            {
                RemoveCallbacks(updateLayerScrollRunnable);
                updateLayerScrollRunnable = null;
            }
        }
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            OnLayoutChildren();
        }
        protected void OnLayoutChildren()
        {
            for (int i = 0; i < ChildCount; i++)
            {
                View child = GetChildAt(i);
                if (child.Visibility == ViewStates.Gone) continue;
                var lp = (ViewLayoutParams)child.LayoutParameters;
                var gravity = lp.Gravity;
                if (gravity == GravityFlags.NoGravity)
                    gravity = GravityFlags.Left | GravityFlags.Top;
                int widthSpace = child.MeasuredWidth + lp.LeftMargin + lp.RightMargin;
                int heightSpace = child.MeasuredHeight + lp.TopMargin + lp.BottomMargin;
                switch (lp.Layer)
                {
                    case ViewLayoutParams.LAYER_CENTER:
                        ContainerRect.Set(CenterRect);
                        ContainerRect.Offset(-LayerScrollX, -LayerScrollY);
                        break;
                    case ViewLayoutParams.LAYER_LEFT:
                        ContainerRect.Set(CenterRect.Left - EdgeSize.Left, CenterRect.Top, CenterRect.Left, CenterRect.Bottom);
                        ContainerRect.Offset(-LayerScrollX, -LayerScrollY);
                        break;
                    case ViewLayoutParams.LAYER_TOP:
                        ContainerRect.Set(CenterRect.Left, CenterRect.Top - EdgeSize.Top, CenterRect.Right, CenterRect.Top);
                        ContainerRect.Offset(-LayerScrollX, -LayerScrollY);
                        break;
                    case ViewLayoutParams.LAYER_RIGHT:
                        ContainerRect.Set(CenterRect.Right, CenterRect.Top, CenterRect.Right + EdgeSize.Right, CenterRect.Bottom);
                        ContainerRect.Offset(-LayerScrollX, -LayerScrollY);
                        break;
                    case ViewLayoutParams.LAYER_BOTTOM:
                        ContainerRect.Set(CenterRect.Left, CenterRect.Bottom, CenterRect.Right, CenterRect.Bottom + EdgeSize.Bottom);
                        ContainerRect.Offset(-LayerScrollX, -LayerScrollY);
                        break;
                    case ViewLayoutParams.LAYER_NONE:
                    default:
                        ContainerRect.Set(CenterRect);
                        break;
                }
                Gravity.Apply(gravity, widthSpace, heightSpace, ContainerRect, OutRect);
                OutRect.Left += lp.LeftMargin;
                OutRect.Top += lp.TopMargin;
                OutRect.Right -= lp.RightMargin;
                OutRect.Bottom -= lp.BottomMargin;
                child.Layout(OutRect.Left, OutRect.Top, OutRect.Right, OutRect.Bottom);
            }
        }
        public long ComputeInfoCode(long offset)
        {
            int count = ChildCount;
            long result = offset;
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                View child = GetChildAt(i);
                result = ComputeHash(result, (int)child.Visibility, index++);
                ViewLayoutParams lp = (ViewLayoutParams)child.LayoutParameters;
                result = ComputeHash(result, lp.Layer, index++);
                result = ComputeHash(result, (int)lp.Gravity, index++);
                result = ComputeHash(result, lp.Width, index++);
                result = ComputeHash(result, lp.Height, index++);
                result = ComputeHash(result, lp.LeftMargin, index++);
                result = ComputeHash(result, lp.TopMargin, index++);
                result = ComputeHash(result, lp.RightMargin, index++);
                result = ComputeHash(result, lp.BottomMargin, index++);
            }
            return result;
        }
        private long ComputeHash(long start, int value, int index)
        {
            return start + (value << index % 32);
        }
        protected virtual bool CanOpenRight(float xv, float yv)
        {
            return (null != CanOpenEdgeCallback && CanOpenEdgeCallback.CanOpenRight(this, xv, yv)) || LayerScrollX >= HorizontalLayerScrollMax;
        }
        protected virtual int HorizontalLayerScrollMin
        {
            get
            {
                return -(EdgeSize.Left - PaddingLeft);
            }
        }
        public bool CanLayerScrollHorizontally(int direction)
        {
            if (direction < 0)
                return LayerScrollX > HorizontalLayerScrollMin;
            else if (direction > 0)
                return LayerScrollX < HorizontalLayerScrollMax;
            return false;
        }
        public bool CanChildrenScrollVertically(MotionEvent @event, int direction)
        {
            for (int i = 0; i < ChildCount; i++)
            {
                int index = ChildCount - 1 - i;
                View child = GetChildAt(index);
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
        protected virtual void OnLayerScrollChanged(int oldX, int oldY, int newX, int newY) { }
        public bool CanChildScrollHorizontally(View child, int direction)
        {
            return null != CanChildScrollCallback && CanChildScrollCallback.CanChildScrollHorizontally(this, child, direction) || child.CanScrollHorizontally(direction);
        }
        public void LayerScrollTo(int x, int y)
        {
            int oldX = this.LayerScrollX;
            int oldY = this.LayerScrollY;
            this.LayerScrollX = x;
            this.LayerScrollY = y;
            RequestLayout();
            OnLayerScrollChanged(oldX, oldY, x, y);
            if (null != OnLayerScrollChangedListener)
                OnLayerScrollChangedListener.OnLayerScrollChanged(this);
        }
        public bool CanChildScrollVertically(View child, int direction)
        {
            return null != CanChildScrollCallback && CanChildScrollCallback.CanChildScrollVertically(this, child, direction) || child.CanScrollVertically(direction);
        }
        protected virtual bool CanVerticalScrollTo(int y)
        {
            if (y < 0)
            {
                if ((overScroll & OVER_SCROLL_TOP) == 0)
                    return y >= VerticalLayerScrollMin;
            }
            else if (y > 0)
            {
                if ((overScroll & OVER_SCROLL_BOTTOM) == 0)
                    return y <= VerticalLayerScrollMax;
            }
            return true;
        }
        protected virtual int HorizontalLayerScrollMax
        {
            get
            {
                return EdgeSize.Right - PaddingRight;
            }
        }
        public bool CanLayerScrollVertically(int direction)
        {
            if (direction < 0)
                return LayerScrollY > VerticalLayerScrollMin;
            else if (direction > 0)
                return LayerScrollY < VerticalLayerScrollMax;
            return false;
        }
        public bool CanChildrenScrollHorizontally(MotionEvent @event, int direction)
        {
            for (int i = 0; i < ChildCount; i++)
            {
                int index = ChildCount - 1 - i;
                View child = GetChildAt(index);
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
        protected virtual int VerticalLayerScrollMax
        {
            get
            {
                return EdgeSize.Bottom - PaddingBottom;
            }
        }
        public void OpenTop(bool smooth)
        {
            PostAfterLayout(new Runnable(() =>
            {
                if (smooth)
                    SmoothLayerScrollTo(0, VerticalLayerScrollMin);
                else
                {
                    if (!layerScroller.IsFinished)
                        layerScroller.AbortAnimation();
                    LayerScrollTo(0, VerticalLayerScrollMin);
                }
            }));
        }
        public void OpenBottom(bool smooth)
        {
            PostAfterLayout(new Runnable(() =>
            {
                if (smooth)
                    SmoothLayerScrollTo(0, VerticalLayerScrollMax);
                else
                {
                    if (!layerScroller.IsFinished)
                        layerScroller.AbortAnimation();
                    LayerScrollTo(0, VerticalLayerScrollMax);
                }
            }));
        }
        public void PostAfterLayout(Runnable runnable)
        {
            if (attached && !IsLayoutRequested)
                runnable.Run();
            else
            {
                if (null == afterLayoutRunnableList)
                    afterLayoutRunnableList = new LinkedList<Runnable>();
                afterLayoutRunnableList.AddLast(runnable);
            }
        }
        protected virtual int VerticalLayerScrollMin
        {
            get
            {
                return -(EdgeSize.Top - PaddingTop);
            }
        }
        protected virtual bool CanOpenTop(float xv, float yv)
        {
            return (null != CanOpenEdgeCallback && CanOpenEdgeCallback.CanOpenTop(this, xv, yv)) || LayerScrollY <= VerticalLayerScrollMin;
        }
        protected virtual bool CanOpenBottom(float xv, float yv)
        {
            return (null != CanOpenEdgeCallback && CanOpenEdgeCallback.CanOpenBottom(this, xv, yv)) || LayerScrollY >= VerticalLayerScrollMax;
        }
        protected virtual bool CanHorizontalScrollTo(int x)
        {
            if (x < 0)
            {
                if ((overScroll & OVER_SCROLL_LEFT) == 0)
                    return x >= HorizontalLayerScrollMin;
            }
            else if (x > 0)
            {
                if ((overScroll & OVER_SCROLL_RIGHT) == 0)
                    return x <= HorizontalLayerScrollMax;
            }
            return true;
        }
        public void SetOverScroll(int overScroll)
        {
            this.overScroll = overScroll;
            RequestLayout();
        }
        private void ReleaseTouch()
        {
            velocityTracker.ComputeCurrentVelocity(1000);
            if (CanOpenLeft(velocityTracker.XVelocity, velocityTracker.YVelocity))
                SmoothLayerScrollTo(HorizontalLayerScrollMin, LayerScrollY);
            else if (CanOpenTop(velocityTracker.XVelocity, velocityTracker.YVelocity))
                SmoothLayerScrollTo(LayerScrollX, VerticalLayerScrollMin);
            else if (CanOpenRight(velocityTracker.XVelocity, velocityTracker.YVelocity))
                SmoothLayerScrollTo(HorizontalLayerScrollMax, LayerScrollY);
            else if (CanOpenBottom(velocityTracker.XVelocity, velocityTracker.YVelocity))
                SmoothLayerScrollTo(LayerScrollX, VerticalLayerScrollMax);
            else
                SmoothLayerScrollTo(0, 0);
        }
    }
}