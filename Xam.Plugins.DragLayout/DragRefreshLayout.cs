using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Xam.Plugins.DragLayout.Interfaces;

namespace Xam.Plugins.DragLayout
{
    public class DragRefreshLayout : DragLayout
    {
        /// <summary>
        /// 周边状态：关闭，不可打开
        /// </summary>
        public const int EDGE_STATE_DISABLE = -1;
        /// <summary>
        /// 周边状态：无，表示无法操作，此状态不能通过滑动更改且
        /// </summary>
        public const int EDGE_STATE_NONE = 0;
        /// <summary>
        /// 周边状态：正常，可以通过滑动更改
        /// </summary>
        public const int EDGE_STATE_NORMAL = 1;
        /// <summary>
        /// 周边状态：准备中，特殊状态，在触摸滑动超出滑动范围后由EDGE_STATE_NORMAL变成的状态，无法手动设置成此状态
        /// </summary>
        public const int EDGE_STATE_READY = 2;
        /// <summary>
        /// 周边状态：读取中，可以由由EDGE_STATE_NORMAL或EDGE_STATE_READY通过滑动变成此状态无法通过滑动取消此状态，必须通过
        /// {@link DragRefreshLayout#setEdgeState(Rect) setEdgeState(Rect)}或{@link DragRefreshLayout#setEdgeState(int, int, int, int) setEdgeState(int, int, int, int)}
        /// 设置状态才能更改。
        /// </summary>
        public const int EDGE_STATE_LOADING = 3;
        /// <summary>
        /// 完成状态：读取完成，手动设置状态，一般都是由EDGE_STATE_LOADING变成
        /// </summary>
        public const int EDGE_STATE_FINISH = 4;
        public Rect EdgeState { get; } = new Rect(EDGE_STATE_NONE, EDGE_STATE_NONE, EDGE_STATE_NONE, EDGE_STATE_NONE);
        private Rect oldEdgeState = new Rect();
        private Rect newEdgeState = new Rect();
        private bool touching = false;
        public IOnEdgeStateChangedListener OnEdgeStateChangedListener { get; set; }
        // 正在滑动的周边，0表示未确定，1表示左边，2表示上边，3表示右边，4表示下边
        private int scrollEdge = 0;
        public DragRefreshLayout(Context context) : base(context)
        {
            Init(null);
        }
        public DragRefreshLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(attrs);
        }
        public DragRefreshLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init(attrs);
        }
        public DragRefreshLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init(attrs);
        }
        private void Init(IAttributeSet attrs) { }
        public override bool OnInterceptTouchEvent(MotionEvent @event)
        {
            if (@event.Action == MotionEventActions.Down)
                touching = true;
            bool result = base.OnInterceptTouchEvent(@event);
            if (@event.Action == MotionEventActions.Cancel || @event.Action == MotionEventActions.Up)
                touching = false;
            return result;
        }
        public override bool PerformClick()
        {
            return base.PerformClick();
        }
        public override bool OnTouchEvent(MotionEvent @event)
        {
            if (@event.Action == MotionEventActions.Down)
            {
                PerformClick();
                touching = true;
            }
            if (@event.Action == MotionEventActions.Cancel || @event.Action == MotionEventActions.Cancel)
                touching = false;
            return base.OnTouchEvent(@event);
        }
        protected override void OnLayerScrollChanged(int oldX, int oldY, int newX, int newY)
        {
            base.OnLayerScrollChanged(oldX, oldY, newX, newY);
            if (touching)
            {
                if (scrollEdge == 0)
                {
                    if (newX < 0)
                        scrollEdge = 1;
                    else if (newX > 0)
                        scrollEdge = 3;
                    else if (newY < 0)
                        scrollEdge = 2;
                    else if (newY > 0)
                        scrollEdge = 4;
                }
            }
            else
            {
                if (newX < 0)
                    scrollEdge = 1;
                else if (newX > 0)
                    scrollEdge = 3;
                else if (newY < 0)
                    scrollEdge = 2;
                else if (newY > 0)
                    scrollEdge = 4;
                else
                    scrollEdge = 0;
            }
            oldEdgeState.Set(EdgeState);
            if (newX <= HorizontalLayerScrollMin)
            {
                if (touching)
                {
                    if (EdgeState.Left == EDGE_STATE_NORMAL)
                        EdgeState.Left = EDGE_STATE_READY;
                }
                else
                {
                    if (EdgeState.Left == EDGE_STATE_NORMAL || EdgeState.Left == EDGE_STATE_READY)
                        EdgeState.Left = EDGE_STATE_LOADING;
                }
            }
            if (newX >= HorizontalLayerScrollMax)
            {
                if (touching)
                {
                    if (EdgeState.Right == EDGE_STATE_NORMAL)
                        EdgeState.Right = EDGE_STATE_READY;
                }
                else
                {
                    if (EdgeState.Right == EDGE_STATE_NORMAL || EdgeState.Right == EDGE_STATE_READY)
                        EdgeState.Right = EDGE_STATE_LOADING;
                }
            }
            if (newX == 0)
            {
                if (EdgeState.Left == EDGE_STATE_FINISH)
                    EdgeState.Left = EDGE_STATE_NORMAL;
                if (EdgeState.Right == EDGE_STATE_FINISH)
                    EdgeState.Right = EDGE_STATE_NORMAL;
            }
            if (newY <= VerticalLayerScrollMin)
            {
                if (touching)
                {
                    if (EdgeState.Top == EDGE_STATE_NORMAL)
                        EdgeState.Top = EDGE_STATE_READY;
                }
                else
                {
                    if (EdgeState.Top == EDGE_STATE_NORMAL || EdgeState.Top == EDGE_STATE_READY)
                        EdgeState.Top = EDGE_STATE_LOADING;
                }
            }
            if (newY >= VerticalLayerScrollMax)
            {
                if (touching)
                {
                    if (EdgeState.Bottom == EDGE_STATE_NORMAL)
                        EdgeState.Bottom = EDGE_STATE_READY;
                }
                else
                {
                    if (EdgeState.Bottom == EDGE_STATE_NORMAL || EdgeState.Bottom == EDGE_STATE_READY)
                        EdgeState.Bottom = EDGE_STATE_LOADING;
                }
            }
            if (newY == 0)
            {
                if (EdgeState.Top == EDGE_STATE_FINISH)
                    EdgeState.Top = EDGE_STATE_NORMAL;
                if (EdgeState.Bottom == EDGE_STATE_FINISH)
                    EdgeState.Bottom = EDGE_STATE_NORMAL;
            }
            if (!oldEdgeState.Equals(EdgeState))
                ChangeEdgeState();
        }
        protected override bool CanHorizontalScrollTo(int x)
        {
            if (IsNoEdgeScroll || (x <= 0 && scrollEdge == 1) || (x >= 0 && scrollEdge == 3))
                return base.CanHorizontalScrollTo(x);
            return false;
        }
        protected override bool CanVerticalScrollTo(int y)
        {
            if (IsNoEdgeScroll || (y <= 0 && scrollEdge == 2) || (y >= 0 && scrollEdge == 4))
                return base.CanVerticalScrollTo(y);
            return false;
        }
        protected override int HorizontalLayerScrollMin
        {
            get
            {
                if (IsRightEdgeScroll || EdgeState.Left == EDGE_STATE_DISABLE)
                    return 0;
                return base.HorizontalLayerScrollMin;
            }
        }
        protected override int HorizontalLayerScrollMax
        {
            get
            {
                if (IsLeftEdgeScroll || EdgeState.Right == EDGE_STATE_DISABLE)
                    return 0;
                return base.HorizontalLayerScrollMax;
            }
        }
        protected override int VerticalLayerScrollMin
        {
            get
            {
                if (IsBottomEdgeScroll || EdgeState.Top == EDGE_STATE_DISABLE)
                    return 0;
                return base.VerticalLayerScrollMin;
            }
        }
        protected override int VerticalLayerScrollMax
        {
            get
            {
                if (IsTopEdgeScroll || EdgeState.Bottom == EDGE_STATE_DISABLE)
                    return 0;
                return base.VerticalLayerScrollMax;
            }
        }
        protected override bool CanOpenTop(float xv, float yv)
        {
            if (EdgeState.Top == EDGE_STATE_DISABLE)
                return false;
            return base.CanOpenTop(xv, yv);
        }
        protected override bool CanOpenLeft(float xv, float yv)
        {
            if (EdgeState.Left == EDGE_STATE_DISABLE)
                return false;
            return base.CanOpenLeft(xv, yv);
        }
        protected override bool CanOpenBottom(float xv, float yv)
        {
            if (EdgeState.Bottom == EDGE_STATE_DISABLE)
                return false;
            return base.CanOpenBottom(xv, yv);
        }
        protected override bool CanOpenRight(float xv, float yv)
        {
            if (EdgeState.Right == EDGE_STATE_DISABLE)
                return false;
            return base.CanOpenRight(xv, yv);
        }
        protected virtual void OnEdgeStateChanged(Rect oldState, Rect state) { }
        public void SetEdgeState(int leftState, int topState, int rightState, int bottomState)
        {
            oldEdgeState.Set(EdgeState);
            this.EdgeState.Set(leftState, topState, rightState, bottomState);
            if (!oldEdgeState.Equals(EdgeState))
                ChangeEdgeState();
        }
        /// <summary>
        /// 设置边缘状态
        /// </summary>
        /// <param name="edgeState">边缘状态</param>
        public void SetEdgeState(Rect edgeState)
        {
            if (null != edgeState)
                SetEdgeState(edgeState.Left, edgeState.Top, edgeState.Right, edgeState.Bottom);
        }
        /// <summary>
        /// 获取边缘状态
        /// </summary>
        /// <returns>边缘状态</returns>
        public Rect EdgeStateTag
        {
            get
            {
                newEdgeState.Set(EdgeState);
                return newEdgeState;
            }
        }
        public bool IsLeftEdgeScroll
        {
            get
            {
                return scrollEdge == 1;
            }
        }
        public bool IsTopEdgeScroll
        {
            get
            {
                return scrollEdge == 2;
            }
        }
        public bool IsRightEdgeScroll
        {
            get
            {
                return scrollEdge == 3;
            }
        }
        public bool IsBottomEdgeScroll
        {
            get
            {
                return scrollEdge == 4;
            }
        }
        public bool IsNoEdgeScroll
        {
            get
            {
                return scrollEdge == 0;
            }
        }
        private void ChangeEdgeState()
        {
            newEdgeState.Set(EdgeState);
            OnEdgeStateChanged(oldEdgeState, newEdgeState);
            if (null != OnEdgeStateChangedListener)
                OnEdgeStateChangedListener.OnEdgeStateChanged(this, oldEdgeState, newEdgeState);
        }
    }
}