using Android.Graphics;
using Android.Views;

namespace Xam.Plugins.Theme
{
    internal static class Extenion
    {
        public static Bitmap ViewDrawingCache(this View view)
        {
            Bitmap bitmap = Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            view.Draw(canvas);
            return bitmap;
        }
    }
}