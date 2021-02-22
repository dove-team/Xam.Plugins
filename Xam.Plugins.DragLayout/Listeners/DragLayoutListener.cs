using Android.Views;

namespace Xam.Plugins.DragLayout.Listeners
{
    public sealed class DragLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private DragLayout DragLayout { get; }
        public DragLayoutListener(DragLayout dragLayout)
        {
            DragLayout = dragLayout;
        }
        public void OnGlobalLayout()
        {
            if (null != DragLayout.afterLayoutRunnableList && DragLayout.afterLayoutRunnableList != null)
            {
                while (DragLayout.afterLayoutRunnableList.Count > 0)
                    DragLayout.afterLayoutRunnableList.RemoveFirst();
            }
        }
    }
}