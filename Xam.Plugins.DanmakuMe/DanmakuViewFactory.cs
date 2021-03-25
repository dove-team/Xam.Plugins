using Android.Content;
using Android.Views;

namespace Xam.Plugins.DanmakuMe
{
    class DanmakuViewFactory
    {
        public static DanmakuView CreateDanmakuView(Context context)
        {
            return (DanmakuView)LayoutInflater.From(context)
                    .Inflate(Resource.Layout.danmaku_view, null, false);
        }
        public static DanmakuView CreateDanmakuView(Context context, ViewGroup parent)
        {
            return (DanmakuView)LayoutInflater.From(context)
                    .Inflate(Resource.Layout.danmaku_view, parent, false);
        }
    }
}