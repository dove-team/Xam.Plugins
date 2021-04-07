using Android.Animation;
using Android.Views;
using Java.Lang;

namespace Xam.Plugins.Theme
{
    public class AnimatorUpdateListener : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
    {
        private View View { get; }
        public AnimatorUpdateListener(View view)
        {
            this.View = view;
        }
        public void OnAnimationUpdate(ValueAnimator animation)
        {
            try
            {
                var alpha = (Float)animation.AnimatedValue;
                View.Alpha = alpha.FloatValue();
            }
            catch { }
        }
    }
}