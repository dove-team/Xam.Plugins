using Android.Views;

namespace Xam.Plugins.ImageViewer
{
    public sealed class GestureDetectorGestureListener : GestureDetector.SimpleOnGestureListener
    {
        private ZoomableTouchListener Listener { get; }
        public GestureDetectorGestureListener(ZoomableTouchListener listener)
        {
            Listener = listener;
        }
        public override bool OnSingleTapConfirmed(MotionEvent e)
        {
            if (Listener.TapListener != null) 
                Listener.TapListener.OnTap(Listener.Target);
            return true;
        }
        public override void OnLongPress(MotionEvent e)
        {
            if (Listener.LongPressListener != null)
                Listener.LongPressListener.OnLongPress(Listener.Target);
        }
        public override bool OnDoubleTap(MotionEvent e)
        {
            if (Listener.DoubleTapListener != null)
                Listener.DoubleTapListener.OnDoubleTap(Listener.Target);
            return true;
        }
    }
}