using Android.Graphics;
using Android.Views;

namespace Xam.Plugins.ImageViewer
{
    public static class ViewUtils
    {
        public static void MidPointOfEvent(this MotionEvent @event,PointF point)
        {
            if (@event.PointerCount == 2)
            {
                float x = @event.GetX(0) + @event.GetX(1);
                float y = @event.GetY(0) + @event.GetY(1);
                point.Set(x / 2, y / 2);
            }
        }
        public static Bitmap GetBitmapFromView(this View view)
        {
            Bitmap returnedBitmap = Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(returnedBitmap);
            view.Draw(canvas);
            return returnedBitmap;
        }
        public static Point GetViewAbsoluteCords(this View v)
        {
            int[] location = new int[2];
            v.GetLocationInWindow(location);
            int x = location[0];
            int y = location[1];
            return new Point(x, y);
        }
    }
}