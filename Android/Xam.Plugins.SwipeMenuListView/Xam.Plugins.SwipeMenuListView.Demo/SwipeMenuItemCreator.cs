using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Java.Lang;

namespace Xam.Plugins.SwipeList.Demo
{
    public sealed class SwipeMenuItemCreator : Object, ISwipeMenuCreator
    {
        private Context Context { get; }
        public SwipeMenuItemCreator(Context context)
        {
            Context = context;
        }
        public void Create(SwipeMenu menu)
        {
            SwipeMenuItem openItem = new SwipeMenuItem(Context);
            openItem.Background = new ColorDrawable(Color.SkyBlue);
            openItem.Width = Context.DpToPx(90);
            openItem.Title = "打开";
            openItem.TitleSize = 18;
            openItem.TitleColor = Color.White;
            menu.AddMenuItem(openItem);
            SwipeMenuItem deleteItem = new SwipeMenuItem(Context);
            deleteItem.Background = new ColorDrawable(Color.IndianRed);
            deleteItem.Width = Context.DpToPx(90);
            deleteItem.Title = "删除";
            openItem.TitleSize = 18;
            openItem.TitleColor = Color.White;
            menu.AddMenuItem(deleteItem);
        }
    }
}