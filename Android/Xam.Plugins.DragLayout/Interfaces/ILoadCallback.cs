namespace Xam.Plugins.DragLayout.Interfaces
{
    public interface ILoadCallback
    {
        void OnRefresh(VerticalRefreshLayout view);
        void OnLoadMore(VerticalRefreshLayout view);
    }
}