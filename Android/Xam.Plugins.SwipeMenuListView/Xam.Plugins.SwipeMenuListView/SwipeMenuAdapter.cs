using Android.Content;
using Android.Database;
using Android.Views;
using Android.Widget;
using JavaObject = Java.Lang.Object;

namespace Xam.Plugins.SwipeList
{
    public class SwipeMenuAdapter : JavaObject, IWrapperListAdapter
    {
        private Context Context { get; }
        private IListAdapter Adapter { get; }
        public event MenuItemClickEvent MenuItemClick;
        public SwipeMenuAdapter(Context context, IListAdapter adapter)
        {
            Context = context;
            Adapter = adapter;
        }
        public int Count => Adapter.Count;
        public bool IsEmpty => Adapter.IsEmpty;
        private SwipeMenuView MenuView { get; set; }
        public bool HasStableIds => Adapter.HasStableIds;
        public IListAdapter WrappedAdapter => Adapter;
        public int ViewTypeCount => Adapter.ViewTypeCount;
        public bool AreAllItemsEnabled() => Adapter.AreAllItemsEnabled();
        public JavaObject GetItem(int position) => Adapter.GetItem(position);
        public long GetItemId(int position) => Adapter.GetItemId(position);
        public int GetItemViewType(int position) => Adapter.GetItemViewType(position);
        public View GetView(int position, View convertView, ViewGroup parent)
        {
            SwipeMenuLayout layout;
            if (convertView == null)
            {
                View contentView = Adapter.GetView(position, convertView, parent);
                SwipeMenu menu = new SwipeMenu(Context);
                menu.ViewType = GetItemViewType(position);
                CreateMenu(menu);
                MenuView = new SwipeMenuView(menu);
                MenuView.ItemClick += MenuView_ItemClick;
                SwipeMenuListView listView = (SwipeMenuListView)parent;
                layout = new SwipeMenuLayout(contentView, MenuView,
                        listView.CloseInterpolator,
                        listView.OpenInterpolator);
                layout.Position = position;
            }
            else
            {
                layout = (SwipeMenuLayout)convertView;
                layout.CloseMenu();
                layout.Position = position;
            }
            if (Adapter is BaseSwipListAdapter adapter)
            {
                var swipEnable = adapter.GetSwipEnableByPosition(position);
                layout.SwipEnable = swipEnable;
            }
            return layout;
        }
        private void MenuView_ItemClick(SwipeMenuView view, SwipeMenu menu, int index)
        {
            OnItemClick(view, menu, index);
        }
        public bool IsEnabled(int position) => Adapter.IsEnabled(position);
        public void RegisterDataSetObserver(DataSetObserver observer) => Adapter.RegisterDataSetObserver(observer);
        public void UnregisterDataSetObserver(DataSetObserver observer) => Adapter.UnregisterDataSetObserver(observer);
        public virtual void CreateMenu(SwipeMenu menu) { }
        public virtual void OnItemClick(SwipeMenuView view, SwipeMenu menu, int index)
        {
            MenuItemClick?.Invoke(view.Position, menu, index);
        }
    }
}