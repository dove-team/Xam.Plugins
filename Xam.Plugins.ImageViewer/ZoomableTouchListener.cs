using Android.Graphics;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using Math = Java.Lang.Math;

namespace Xam.Plugins.ImageViewer
{
    public sealed class ZoomableTouchListener : Object, View.IOnTouchListener, ScaleGestureDetector.IOnScaleGestureListener
    {
        internal const int STATE_IDLE = 0;
        private const int STATE_POINTER_DOWN = 1;
        private const int STATE_ZOOMING = 2;
        private const float MIN_SCALE_FACTOR = 1f;
        private const float MAX_SCALE_FACTOR = 5f;
        internal int State = STATE_IDLE;
        internal ITapListener TapListener { get; }
        internal IZoomListener ZoomListener { get; }
        private IRunnable EndingZoomAction { get; }
        internal ITargetContainer TargetContainer { get; }
        internal ILongPressListener LongPressListener { get; }
        private IInterpolator EndZoomingInterpolator { get; }
        internal IDoubleTapListener DoubleTapListener { get; }
        internal View Target { get; }
        internal View Shadow { get; set; }
        internal ViewerConfig Config { get; }
        internal ImageView ZoomableView { get; set; }
        private GestureDetector GestureDetector { get; }
        private Point TargetViewCords { get; set; } = new Point();
        private ScaleGestureDetector ScaleGestureDetector { get; }
        internal PointF InitialPinchMidPoint { get; set; } = new PointF();
        internal PointF CurrentMovementMidPoint { get; set; } = new PointF();
        private GestureDetector.SimpleOnGestureListener GestureListener { get; }
        private float ScaleFactor = 1f;
        internal bool AnimatingZoomEnding = false;
        public ZoomableTouchListener(ITargetContainer targetContainer, View view, ViewerConfig config, IInterpolator interpolator, IZoomListener zoomListener, ITapListener tapListener, ILongPressListener longPressListener, IDoubleTapListener doubleTapListener)
        {
            this.TargetContainer = targetContainer;
            this.Target = view;
            this.Config = config;
            this.EndZoomingInterpolator = interpolator ?? new AccelerateDecelerateInterpolator();
            this.ScaleGestureDetector = new ScaleGestureDetector(view.Context, this);
            this.GestureDetector = new GestureDetector(view.Context, GestureListener);
            this.ZoomListener = zoomListener;
            this.TapListener = tapListener;
            this.LongPressListener = longPressListener;
            this.DoubleTapListener = doubleTapListener;
            GestureListener = new GestureDetectorGestureListener(this);
            EndingZoomAction = new ZoomAction(this);
        }
        public bool OnScale(ScaleGestureDetector detector)
        {
            if (ZoomableView == null) return false;
            ScaleFactor *= detector.ScaleFactor;
            ScaleFactor = Math.Max(MIN_SCALE_FACTOR, Math.Min(ScaleFactor, MAX_SCALE_FACTOR));
            ZoomableView.ScaleX = ScaleFactor;
            ZoomableView.ScaleY = ScaleFactor;
            ObscureDecorView(ScaleFactor);
            return true;
        }
        public bool OnScaleBegin(ScaleGestureDetector detector)
        {
            return ZoomableView != null;
        }
        public void OnScaleEnd(ScaleGestureDetector detector)
        {
            ScaleFactor = 1f;
        }
        public bool OnTouch(View v, MotionEvent ev)
        {
            if (AnimatingZoomEnding || ev.PointerCount > 2) return true;
            ScaleGestureDetector.OnTouchEvent(ev);
            GestureDetector.OnTouchEvent(ev);
            var action = ev.Action & MotionEventActions.Mask;
            switch (action)
            {
                case MotionEventActions.PointerDown:
                case MotionEventActions.Down:
                    switch (State)
                    {
                        case STATE_IDLE:
                            State = STATE_POINTER_DOWN;
                            break;
                        case STATE_POINTER_DOWN:
                            State = STATE_ZOOMING;
                            ev.MidPointOfEvent(InitialPinchMidPoint);
                            StartZoomingView(Target);
                            break;
                    }
                    break;
                case MotionEventActions.Move:
                    if (State == STATE_ZOOMING)
                    {
                        ev.MidPointOfEvent(CurrentMovementMidPoint);
                        CurrentMovementMidPoint.X -= InitialPinchMidPoint.X;
                        CurrentMovementMidPoint.Y -= InitialPinchMidPoint.Y;
                        CurrentMovementMidPoint.X += TargetViewCords.X;
                        CurrentMovementMidPoint.Y += TargetViewCords.Y;
                        float x = CurrentMovementMidPoint.X;
                        float y = CurrentMovementMidPoint.Y;
                        ZoomableView.SetX(x);
                        ZoomableView.SetY(y);
                    }
                    break;
                case MotionEventActions.PointerUp:
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    switch (State)
                    {
                        case STATE_ZOOMING:
                            EndZoomingView();
                            break;
                        case STATE_POINTER_DOWN:
                            State = STATE_IDLE;
                            break;
                    }
                    break;
            }
            return true;
        }
        private void EndZoomingView()
        {
            if (Config.ZoomAnimationEnabled)
            {
                AnimatingZoomEnding = true;
                ZoomableView.Animate().X(TargetViewCords.X).Y(TargetViewCords.Y).ScaleX(1).ScaleY(1).SetInterpolator(EndZoomingInterpolator).WithEndAction(EndingZoomAction).Start();
            }
            else
                EndingZoomAction.Run();
        }
        private void StartZoomingView(View view)
        {
            ZoomableView = new ImageView(Target.Context) { LayoutParameters = new ViewGroup.LayoutParams(Target.Width, Target.Height) };
            ZoomableView.SetImageBitmap(view.GetBitmapFromView());
            TargetViewCords = view.GetViewAbsoluteCords();
            ZoomableView.SetX(TargetViewCords.X);
            ZoomableView.SetY(TargetViewCords.Y);
            if (Shadow == null)
                Shadow = new View(Target.Context);
            Shadow.SetBackgroundResource(0);
            AddToDecorView(Shadow);
            AddToDecorView(ZoomableView);
            DisableParentTouch(Target.Parent);
            Target.Visibility = ViewStates.Invisible;
            if (Config.ImmersiveModeEnabled)
                HideSystemUI();
            if (ZoomListener != null)
                ZoomListener.OnViewStartedZooming(Target);
        }
        private void AddToDecorView(View v)
        {
            TargetContainer.DecorView.AddView(v);
        }
        internal void RemoveFromDecorView(View v)
        {
            TargetContainer.DecorView.RemoveView(v);
        }
        private void ObscureDecorView(float factor)
        {
            float normalizedValue = (factor - MIN_SCALE_FACTOR) / (MAX_SCALE_FACTOR - MIN_SCALE_FACTOR);
            normalizedValue = Math.Min(0.75f, normalizedValue * 2);
            var obscure = Color.Argb((int)(normalizedValue * 255), 0, 0, 0);
            Shadow.SetBackgroundColor(obscure);
        }
        private void HideSystemUI()
        {
            TargetContainer.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;
        }
        internal void ShowSystemUI()
        {
            TargetContainer.DecorView.SystemUiVisibility = StatusBarVisibility.Visible;
        }
        private void DisableParentTouch(IViewParent view)
        {
            view.RequestDisallowInterceptTouchEvent(true);
            if (view.Parent != null)
                DisableParentTouch((view.Parent));
        }
    }
}