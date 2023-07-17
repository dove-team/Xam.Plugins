using Android.Widget;

namespace Xam.Plugins.SwipeList
{
    public sealed class DefaultSwipeMenuAdapter : SwipeMenuAdapter
    {
        private SwipeMenuListView Owner { get; }
        public DefaultSwipeMenuAdapter(SwipeMenuListView view, IListAdapter adapter) : base(view.Context, adapter)
        {
            Owner = view;
        }
        public override void CreateMenu(SwipeMenu menu)
        {
            if (Owner.MenuCreator != null)
                Owner.MenuCreator.Create(menu);
        }
        public override void OnItemClick(SwipeMenuView view, SwipeMenu menu, int index)
        {
            Owner.OnMenuItemClick(view.Position, menu, index);
            if (Owner.TouchView != null)
                Owner.TouchView.SmoothCloseMenu();
        }
    }
}