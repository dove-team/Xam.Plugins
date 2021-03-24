using Android.Graphics;
using Android.Views;
using Java.Lang;

namespace Xam.Plugins.ImageViewer
{
    public sealed class ZoomAction : Object, IRunnable
    {
        private ZoomableTouchListener Listener { get; }
        public ZoomAction(ZoomableTouchListener listener)
        {
            Listener = listener;
        }
        public void Run()
        {
            Listener.RemoveFromDecorView(Listener.Shadow);
            Listener.RemoveFromDecorView(Listener.ZoomableView);
            Listener.Target.Visibility = ViewStates.Visible;
            Listener.ZoomableView = null;
            Listener.CurrentMovementMidPoint = new PointF();
            Listener.InitialPinchMidPoint = new PointF();
            Listener.AnimatingZoomEnding = false;
            Listener.State = ZoomableTouchListener.STATE_IDLE;
            if (Listener.ZoomListener != null) 
                Listener.ZoomListener.OnViewEndedZooming(Listener.Target);
            if (Listener.Config.ImmersiveModeEnabled)
                Listener.ShowSystemUI();
        }
    }
}