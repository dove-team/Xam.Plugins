using Android.Animation;
using Android.Views;

namespace Xam.Plugins.Theme
{
    public class AnimatorListenerAdapter : Android.Animation.AnimatorListenerAdapter
    {
        private View View { get; }
        private View DecorView { get; }
        public AnimatorListenerAdapter(View decorView, View view)
        {
            this.DecorView = decorView;
            this.View = view;
        }
        public override void OnAnimationEnd(Animator animation)
        {
            base.OnAnimationEnd(animation);
            ((ViewGroup)DecorView).RemoveView(View);
        }
    }
}