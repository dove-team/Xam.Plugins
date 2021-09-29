using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System;
using Math = Java.Lang.Math;

namespace Xam.Plugins.SwipeList
{
    public class SwipeMenuListView : ListView
    {
        private const int TOUCH_STATE_NONE = 0;
        private const int TOUCH_STATE_X = 1;
        private const int TOUCH_STATE_Y = 2;
        public const int DIRECTION_LEFT = 1;
        public const int DIRECTION_RIGHT = -1;
        private float DownX;
        private float DownY;
        private int TouchState;
        private int MAX_Y = 5;
        private int MAX_X = 3;
        private int Direction = 1;
        private int TouchPosition;
        public SwipeMenuLayout TouchView { get; private set; }
        public event SwipeEndEvent SwipeEnd;
        public event SwipeStartEvent SwipeStart;
        public event MenuOpenEvent MenuOpen;
        public event MenuCloseEvent MenuClose;
        public event MenuItemClickEvent MenuItemClick;
        public void OnMenuItemClick(int position, SwipeMenu menu, int index)
        {
            MenuItemClick?.Invoke(position, menu, index);
        }
        public IInterpolator CloseInterpolator { get; private set; }
        public IInterpolator OpenInterpolator { get; private set; }
        public ISwipeMenuCreator MenuCreator { get; set; }
        public SwipeMenuListView(Context context) : base(context) => Init();
        public SwipeMenuListView(Context context, IAttributeSet attrs) : base(context, attrs) => Init();
        public SwipeMenuListView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) => Init();
        public SwipeMenuListView(IntPtr reference, JniHandleOwnership transfer) : base(reference, transfer) { }
        private void Init()
        {
            MAX_X = Context.DpToPx(MAX_X);
            MAX_Y = Context.DpToPx(MAX_Y);
            TouchState = TOUCH_STATE_NONE;
        }
        public override IListAdapter Adapter
        {
            get => base.Adapter;
            set => base.Adapter = new DefaultSwipeMenuAdapter(this, value);
        }
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            switch (ev.Action)
            {
                case MotionEventActions.Down:
                    {
                        DownX = ev.GetX();
                        DownY = ev.GetY();
                        var handled = base.OnInterceptTouchEvent(ev);
                        TouchState = TOUCH_STATE_NONE;
                        TouchPosition = PointToPosition((int)ev.GetX(), (int)ev.GetY());
                        View view = GetChildAt(TouchPosition - FirstVisiblePosition);
                        if (view is SwipeMenuLayout layout)
                        {
                            if (TouchView != null && TouchView.IsOpen && !InRangeOfView(TouchView.MenuView, ev))
                                return true;
                            TouchView = layout;
                            TouchView.SwipeDirection = Direction;
                        }
                        if (TouchView != null && TouchView.IsOpen && view != TouchView)
                            handled = true;
                        if (TouchView != null)
                            TouchView.OnSwipe(ev);
                        return handled;
                    }
                case MotionEventActions.Move:
                    {
                        float dy = Math.Abs((ev.GetY() - DownY));
                        float dx = Math.Abs((ev.GetX() - DownX));
                        if (Math.Abs(dy) > MAX_Y || Math.Abs(dx) > MAX_X)
                        {
                            if (TouchState == TOUCH_STATE_NONE)
                            {
                                if (Math.Abs(dy) > MAX_Y)
                                    TouchState = TOUCH_STATE_Y;
                                else if (dx > MAX_X)
                                {
                                    TouchState = TOUCH_STATE_X;
                                    SwipeStart?.Invoke(TouchPosition);
                                }
                            }
                            return true;
                        }
                        break;
                    }
            }
            return base.OnInterceptTouchEvent(ev);
        }
        public override bool OnTouchEvent(MotionEvent ev)
        {
            if (ev.Action != MotionEventActions.Down && TouchView == null)
                return base.OnTouchEvent(ev);
            switch (ev.Action)
            {
                case MotionEventActions.Down:
                    int oldPos = TouchPosition;
                    DownX = ev.GetX();
                    DownY = ev.GetY();
                    TouchState = TOUCH_STATE_NONE;
                    TouchPosition = PointToPosition((int)ev.GetX(), (int)ev.GetY());
                    if (TouchPosition == oldPos && TouchView != null && TouchView.IsOpen)
                    {
                        TouchState = TOUCH_STATE_X;
                        TouchView.OnSwipe(ev);
                        return true;
                    }
                    View view = GetChildAt(TouchPosition - FirstVisiblePosition);
                    if (TouchView != null && TouchView.IsOpen)
                    {
                        TouchView.SmoothCloseMenu();
                        TouchView = null;
                        MotionEvent cancelEvent = MotionEvent.Obtain(ev);
                        cancelEvent.Action = MotionEventActions.Cancel;
                        OnTouchEvent(cancelEvent);
                        MenuClose?.Invoke(oldPos);
                        return true;
                    }
                    if (view is SwipeMenuLayout layout)
                    {
                        TouchView = layout;
                        TouchView.SwipeDirection = Direction;
                    }
                    if (TouchView != null)
                        TouchView.OnSwipe(ev);
                    break;
                case MotionEventActions.Move:
                    TouchPosition = PointToPosition((int)ev.GetX(), (int)ev.GetY()) - HeaderViewsCount;
                    if (!TouchView.SwipEnable || TouchPosition != TouchView.Position)
                        break;
                    float dy = Math.Abs(ev.GetY() - DownY);
                    float dx = Math.Abs(ev.GetX() - DownX);
                    if (TouchState == TOUCH_STATE_X)
                    {
                        if (TouchView != null)
                            TouchView.OnSwipe(ev);
                        Selector.SetState(new int[] { 0 });
                        ev.Action = MotionEventActions.Cancel;
                        base.OnTouchEvent(ev);
                        return true;
                    }
                    else if (TouchState == TOUCH_STATE_NONE)
                    {
                        if (Math.Abs(dy) > MAX_Y)
                            TouchState = TOUCH_STATE_Y;
                        else if (dx > MAX_X)
                        {
                            TouchState = TOUCH_STATE_X;
                            SwipeStart?.Invoke(TouchPosition);
                        }
                    }
                    break;
                case MotionEventActions.Up:
                    if (TouchState == TOUCH_STATE_X)
                    {
                        if (TouchView != null)
                        {
                            bool isBeforeOpen = TouchView.IsOpen;
                            TouchView.OnSwipe(ev);
                            var isAfterOpen = TouchView.IsOpen;
                            if (isBeforeOpen != isAfterOpen)
                            {
                                if (isAfterOpen)
                                    MenuOpen?.Invoke(TouchPosition);
                                else
                                    MenuClose?.Invoke(TouchPosition);
                            }
                            if (!isAfterOpen)
                            {
                                TouchPosition = -1;
                                TouchView = null;
                            }
                        }
                        SwipeEnd?.Invoke(TouchPosition);
                        ev.Action = MotionEventActions.Cancel;
                        base.OnTouchEvent(ev);
                        return true;
                    }
                    break;
            }
            return base.OnTouchEvent(ev);
        }
        public void SmoothOpenMenu(int position)
        {
            if (position >= FirstVisiblePosition && position <= LastVisiblePosition)
            {
                View view = GetChildAt(position - FirstVisiblePosition);
                if (view is SwipeMenuLayout layout)
                {
                    TouchPosition = position;
                    if (TouchView != null && TouchView.IsOpen)
                        TouchView.SmoothCloseMenu();
                    TouchView = layout;
                    TouchView.SwipeDirection = Direction;
                    TouchView.SmoothOpenMenu();
                }
            }
        }
        public void SmoothCloseMenu()
        {
            if (TouchView != null && TouchView.IsOpen)
                TouchView.SmoothCloseMenu();
        }
        public bool InRangeOfView(View view, MotionEvent ev)
        {
            int[] location = new int[2];
            view.GetLocationOnScreen(location);
            int x = location[0], y = location[1];
            return ev.RawX >= x && ev.RawX <= (x + view.Width) && ev.RawY >= y && ev.RawY <= (y + view.Height);
        }
    }
}