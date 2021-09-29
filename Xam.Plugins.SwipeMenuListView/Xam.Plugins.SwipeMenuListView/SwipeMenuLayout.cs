using Android.Content;
using Android.Support.V4.View;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Math = Java.Lang.Math;

namespace Xam.Plugins.SwipeList
{
	public class SwipeMenuLayout : FrameLayout
	{
		private const int CONTENT_VIEW_ID = 1;
		private const int MENU_VIEW_ID = 2;
		private const int STATE_CLOSE = 0;
		private const int STATE_OPEN = 1;
		private int DownX;
		private int State = STATE_CLOSE;
		private GestureDetectorCompat GestureDetector;
		internal bool IsFling { get; set; }
		internal int MIN_FLING;
		internal int MAX_VELOCITYX;
		private OverScroller OpenScroller;
		private OverScroller CloseScroller;
		private IInterpolator CloseInterpolator;
		private IInterpolator OpenInterpolator;
		public bool SwipEnable { get; set; } = true;
		private GestureDetector.IOnGestureListener GestureListener;
		public View ContentView { get; private set; }
		public SwipeMenuView MenuView { get; private set; }
		private int mBaseX;
		private int position;
		public int Position
		{
			get => position;
			set
			{
				position = value;
				MenuView.Position = position;
			}
		}
		public int SwipeDirection { get; set; }
		public SwipeMenuLayout(View contentView, SwipeMenuView menuView) : this(contentView, menuView, null, null) { }
		public SwipeMenuLayout(View contentView, SwipeMenuView menuView, IInterpolator closeInterpolator, IInterpolator openInterpolator) : base(contentView.Context)
		{
			MIN_FLING = Context.DpToPx(15);
			MAX_VELOCITYX = Context.DpToPx(15);
			CloseInterpolator = closeInterpolator;
			OpenInterpolator = openInterpolator;
			ContentView = contentView;
			MenuView = menuView;
			MenuView.ViewLayout = this;
			Init();
		}
		private void Init()
		{
			LayoutParameters = new AbsListView.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
			GestureListener = new SwipeOnGestureListener(this);
			GestureDetector = new GestureDetectorCompat(Context, GestureListener);
			CloseScroller = CloseInterpolator != null ? new OverScroller(Context, CloseInterpolator) : new OverScroller(Context);
			OpenScroller = OpenInterpolator != null ? new OverScroller(Context, OpenInterpolator) : new OverScroller(Context);
			var contentParams = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
			ContentView.LayoutParameters = contentParams;
			if (ContentView.Id < 1)
				ContentView.Id = CONTENT_VIEW_ID;
			MenuView.Id = MENU_VIEW_ID;
			MenuView.LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
			AddView(ContentView);
			AddView(MenuView);
		}
		public bool OnSwipe(MotionEvent e)
		{
			GestureDetector.OnTouchEvent(e);
			switch (e.Action)
			{
				case MotionEventActions.Down:
					DownX = (int)e.GetX();
					IsFling = false;
					break;
				case MotionEventActions.Move:
					int dis = (int)(DownX - e.GetX());
					if (State == STATE_OPEN)
						dis += MenuView.Width * SwipeDirection;
					Swipe(dis);
					break;
				case MotionEventActions.Up:
					if ((IsFling || Math.Abs(DownX - e.GetX()) > (MenuView.Width / 2)) && Math.Signum(DownX - e.GetX()) == SwipeDirection)
						SmoothOpenMenu();
					else
					{
						SmoothCloseMenu();
						return false;
					}
					break;
			}
			return true;
		}
		public bool IsOpen => State == STATE_OPEN;
		private void Swipe(int dis)
		{
			if (!SwipEnable) return;
			if (Math.Signum(dis) != SwipeDirection) dis = 0;
			else if (Math.Abs(dis) > MenuView.Width)
				dis = MenuView.Width * SwipeDirection;
			ContentView.Layout(-dis, ContentView.Top, ContentView.Width - dis, MeasuredHeight);
			if (SwipeDirection == SwipeMenuListView.DIRECTION_LEFT)
				MenuView.Layout(ContentView.Width - dis, MenuView.Top, ContentView.Width + MenuView.Width - dis, MenuView.Bottom);
			else
				MenuView.Layout(-MenuView.Width - dis, MenuView.Top, -dis, MenuView.Bottom);
		}
		public void SmoothCloseMenu()
		{
			State = STATE_CLOSE;
			if (SwipeDirection == SwipeMenuListView.DIRECTION_LEFT)
			{
				mBaseX = -ContentView.Left;
				CloseScroller.StartScroll(0, 0, MenuView.Width, 0, 350);
			}
			else
			{
				mBaseX = MenuView.Right;
				CloseScroller.StartScroll(0, 0, MenuView.Width, 0, 350);
			}
			PostInvalidate();
		}
		public void SmoothOpenMenu()
		{
			if (!SwipEnable) return;
			State = STATE_OPEN;
			if (SwipeDirection == SwipeMenuListView.DIRECTION_LEFT)
				OpenScroller.StartScroll(-ContentView.Left, 0, MenuView.Width, 0, 350);
			else
				OpenScroller.StartScroll(ContentView.Left, 0, MenuView.Width, 0, 350);
			PostInvalidate();
		}
		public void CloseMenu()
		{
			if (CloseScroller.ComputeScrollOffset())
				CloseScroller.AbortAnimation();
			if (State == STATE_OPEN)
			{
				State = STATE_CLOSE;
				Swipe(0);
			}
		}
		public void OpenMenu()
		{
			if (!SwipEnable) return;
			if (State == STATE_CLOSE)
			{
				State = STATE_OPEN;
				Swipe(MenuView.Width * SwipeDirection);
			}
		}
		public override void ComputeScroll()
		{
			if (State == STATE_OPEN)
			{
				if (OpenScroller.ComputeScrollOffset())
				{
					Swipe(OpenScroller.CurrX * SwipeDirection);
					PostInvalidate();
				}
			}
			else
			{
				if (CloseScroller.ComputeScrollOffset())
				{
					Swipe((mBaseX - CloseScroller.CurrX) * SwipeDirection);
					PostInvalidate();
				}
			}
		}
		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
			MenuView.Measure(MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified),
					MeasureSpec.MakeMeasureSpec(MeasuredHeight, MeasureSpecMode.Exactly));
		}
		protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
		{
			ContentView.Layout(0, 0, MeasuredWidth, ContentView.MeasuredHeight);
			if (SwipeDirection == SwipeMenuListView.DIRECTION_LEFT)
				MenuView.Layout(MeasuredWidth, 0, MeasuredWidth + MenuView.MeasuredWidth, ContentView.MeasuredHeight);
			else
				MenuView.Layout(-MenuView.MeasuredWidth, 0, 0, ContentView.MeasuredHeight);
		}
		public void SetMenuHeight(int measuredHeight)
		{
			LayoutParams layoutParams = (LayoutParams)MenuView.LayoutParameters;
			if (layoutParams.Height != measuredHeight)
			{
				layoutParams.Height = measuredHeight;
				MenuView.LayoutParameters = MenuView.LayoutParameters;
			}
		}
	}
}