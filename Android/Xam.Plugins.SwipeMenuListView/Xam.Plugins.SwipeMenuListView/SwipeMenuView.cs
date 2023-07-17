using Android.Text;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace Xam.Plugins.SwipeList
{
	public class SwipeMenuView : LinearLayout, View.IOnClickListener
	{
		public int Position { get; set; }
		private SwipeMenu Menu { get; }
		public event ItemClickEvent ItemClick;
		public SwipeMenuLayout ViewLayout { get; set; }
		public SwipeMenuView(SwipeMenu menu) : base(menu.Context)
		{
			Menu = menu;
			List<SwipeMenuItem> items = menu.MenuItems;
			int id = 0;
			foreach (var item in items)
				AddItem(item, id++);
		}
		private void AddItem(SwipeMenuItem item, int id)
		{
			LayoutParams layoutParams = new LayoutParams(item.Width, LayoutParams.MatchParent);
			LinearLayout parent = new LinearLayout(Context)
			{
				Id = id,
				Orientation = Orientation.Vertical,
				LayoutParameters = layoutParams,
				Background = item.Background
			};
			parent.SetGravity(GravityFlags.Center);
			parent.SetOnClickListener(this);
			AddView(parent);
			if (item.Icon != null)
				parent.AddView(CreateIcon(item));
			if (!TextUtils.IsEmpty(item.Title))
				parent.AddView(CreateTitle(item));
		}
		private ImageView CreateIcon(SwipeMenuItem item)
		{
			ImageView iv = new ImageView(Context);
			iv.SetImageDrawable(item.Icon);
			return iv;
		}
		private TextView CreateTitle(SwipeMenuItem item)
		{
			TextView tv = new TextView(Context)
			{
				Text = item.Title,
				Gravity = GravityFlags.Center,
				TextSize = item.TitleSize
			};
			tv.SetTextColor(item.TitleColor);
			return tv;
		}
		public void OnClick(View v)
		{
			if (ViewLayout.IsOpen)
				ItemClick?.Invoke(this, Menu, v.Id);
		}
	}
}