using Android.Views;

namespace Xam.Plugins.DragLayout.Interfaces
{
    public interface ICanChildScrollCallback
    {
        bool CanChildScrollVertically(DragLayout view, View child, int direction);
        bool CanChildScrollHorizontally(DragLayout view, View child, int direction);
    }
}