using Android.Views;

namespace Xam.Plugins.ViewUtils
{
    public abstract class ViewScrollHelper : ScrollHelper
    {
        public View View { get; protected set; }
        public ViewScrollHelper(View view, GestureHelper gestureHelper) : base(gestureHelper)
        {
            this.View = view;
        }
        public ViewScrollHelper(View view) : base(view.Context)
        {
            this.View = view;
        }
        protected override int GetViewHorizontallyScrollSize()
        {
            return default;
        }
        protected override int GetViewScrollX()
        {
            return View.ScrollX;
        }
        protected override int GetViewScrollY()
        {
            return View.ScrollY;
        }
        protected override int GetViewVerticallyScrollSize()
        {
            return default;
        }
        protected override void ViewScrollTo(int x, int y)
        {
            View.ScrollTo(x, y);
        }
        protected override void ViewFling(float xv, float yv) { }
    }
}