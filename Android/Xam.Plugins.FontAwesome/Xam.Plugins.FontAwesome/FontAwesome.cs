using Android.App;
using Android.Graphics;
using Android.Support.V4.Content.Res;
using Android.Views;
using Android.Widget;

namespace Xam.Plugins.FontAwesome
{
    public sealed class FontAwesome
    {
        private Typeface Font { get; set; }
        private static FontAwesome instance;
        public static FontAwesome Instance
        {
            get
            {
                if (instance == null)
                    instance = new FontAwesome();
                return instance;
            }
        }
        private FontAwesome() { }
        public void Init(Activity activity)
        {
            if (Font == null)
                Font = ResourcesCompat.GetFont(activity, Resource.Font.fontawesome);
        }
        public void Draw(TextView textView, string code)
        {
            if (textView != null && Font != null)
            {
                textView.Typeface = Font;
                textView.Text = code;
            }
        }
        public void Draw(Activity activity, int textViewId, string code)
        {
            TextView textView = activity.FindViewById<TextView>(textViewId);
            Draw(textView, code);
        }
        public void Draw(View view, int textViewId, string code)
        {
            TextView textView = view.FindViewById<TextView>(textViewId);
            Draw(textView, code);
        }
    }
}