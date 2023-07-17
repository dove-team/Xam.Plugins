using Android.Views;

namespace Xam.Plugins.ImageViewer
{
    public interface IDoubleTapListener
    {
        void OnDoubleTap(View view);
    }
    public interface ILongPressListener
    {
        void OnLongPress(View view);
    }
    public interface ITapListener
    {
        void OnTap(View view);
    }
    public interface ITargetContainer
    {
        ViewGroup DecorView { get; }
    }
    public interface IZoomListener
    {
        void OnViewStartedZooming(View view);
        void OnViewEndedZooming(View view);
    }
}