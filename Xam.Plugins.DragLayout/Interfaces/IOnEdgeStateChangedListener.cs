using Android.Graphics;

namespace Xam.Plugins.DragLayout.Interfaces
{
    /// <summary>
    /// 边缘状态更改监听器
    /// </summary>
    public interface IOnEdgeStateChangedListener
    {
        void OnEdgeStateChanged(DragRefreshLayout view, Rect oldState, Rect state);
    }
}