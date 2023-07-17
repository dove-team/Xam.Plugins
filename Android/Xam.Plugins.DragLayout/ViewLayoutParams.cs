using Android.Content;
using Android.Content.Res;
using Android.Util;
using Android.Views;
using static Android.Views.ViewGroup;

namespace Xam.Plugins.DragLayout
{
    public class ViewLayoutParams : MarginLayoutParams
    {
        /// <summary>
        /// 层：无，不受滑动影响，效果相当于将视图放在FrameLayout中
        /// </summary>
        public const int LAYER_NONE = 0;
        /// <summary>
        /// 层：中间，表示放置在布局中间滑动层
        /// </summary>
        public const int LAYER_CENTER = 1;
        /// <summary>
        /// 层：左边，表示放置在布局左边滑动层
        /// </summary>
        public const int LAYER_LEFT = 2;
        /// <summary>
        /// 层：上面，表示放置在布局上面滑动层
        /// </summary>
        public const int LAYER_TOP = 3;
        /// <summary>
        /// 层：中间，表示放置在布局右边滑动层
        /// </summary>
        public const int LAYER_RIGHT = 4;
        /// <summary>
        /// 层：中间，表示放置在布局下面滑动层
        /// </summary>
        public const int LAYER_BOTTOM = 5;
        /// <summary>
        /// 表示视图位于的层
        /// </summary>
        public int Layer = LAYER_NONE;
        public GravityFlags Gravity;
        public ViewLayoutParams(Context c, IAttributeSet attrs) : base(c, attrs)
        {
            TypedArray ta = c.ObtainStyledAttributes(attrs, Resource.Styleable.DragLayout_Layout);
            this.Layer = ta.GetInt(Resource.Styleable.DragLayout_Layout_layout_layer, LAYER_NONE);
            this.Gravity = (GravityFlags)ta.GetInt(Resource.Styleable.DragLayout_Layout_android_layout_gravity, (int)GravityFlags.NoGravity);
            ta.Recycle();
        }
        public ViewLayoutParams(int width, int height) : base(width, height) { }
        public ViewLayoutParams(ViewGroup.LayoutParams source) : base(source) { }
        public ViewLayoutParams(MarginLayoutParams source) : base(source) { }
        public ViewLayoutParams(ViewLayoutParams source) : base(source)
        {
            this.Gravity = source.Gravity;
            this.Layer = source.Layer;
        }
    }
}