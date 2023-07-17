using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;

namespace Xam.Plugins.SwipeList
{
    public class SwipeMenuItem
	{
		public int Id { get; set; }
		public int Width { get; set; }
		public string Title { get; set; }
		public int TitleSize { get; set; }
		public Context Context { get; }
		public Drawable Icon { get; set; }
		public Color TitleColor { get; set; }
		public Drawable Background { get; set; }
		public SwipeMenuItem(Context context)
		{
			Context = context;
		}
		public void SetTitle(int resId)
		{
			Title = Context.GetString(resId);
		}
		public void SetIcon(int resId)
		{
			Icon = ContextCompat.GetDrawable(Context, resId);
		}
		public void SetBackground(int resId)
		{
			Background = ContextCompat.GetDrawable(Context, resId);
		}
	}
}