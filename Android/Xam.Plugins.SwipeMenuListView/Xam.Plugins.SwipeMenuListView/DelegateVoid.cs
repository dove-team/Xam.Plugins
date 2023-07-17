namespace Xam.Plugins.SwipeList
{
    public delegate void SwipeEndEvent(int position);
    public delegate void MenuCloseEvent(int position);
    public delegate void MenuOpenEvent(int position);
    public delegate void SwipeStartEvent(int position);
    public delegate void MenuItemClickEvent(int position, SwipeMenu menu, int index);
    public delegate void ItemClickEvent(SwipeMenuView view, SwipeMenu menu, int index);
}