using Android.Content;
using System.Collections.Generic;

namespace Xam.Plugins.SwipeList
{
    public class SwipeMenu
	{
		public int ViewType { get; set; }
		public Context Context { get; private set; }
		public List<SwipeMenuItem> MenuItems { get; private set; }
		public SwipeMenu(Context context)
		{
			Context = context;
			MenuItems = new List<SwipeMenuItem>();
		}
		public void AddMenuItem(SwipeMenuItem item)
		{
			MenuItems.Add(item);
		}
		public void RemoveMenuItem(SwipeMenuItem item)
		{
			MenuItems.Remove(item);
		}
	}
}