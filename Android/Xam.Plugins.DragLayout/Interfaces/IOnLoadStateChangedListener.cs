namespace Xam.Plugins.DragLayout.Interfaces
{
    public interface IOnLoadStateChangedListener
    {
        void OnRefreshStateChanged(VerticalRefreshLayout view, int oldState, int state);
        void OnLoadMoreStateChanged(VerticalRefreshLayout view, int oldState, int state);
    }
}