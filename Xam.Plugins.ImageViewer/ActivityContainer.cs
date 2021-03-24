using Android.App;
using Android.Views;

namespace Xam.Plugins.ImageViewer
{
    public class ActivityContainer : ITargetContainer
    {
        private Activity Activity { get; }
        public ViewGroup DecorView
        {
            get
            {
                return (ViewGroup)Activity.Window.DecorView;
            }
        }
        public ActivityContainer(Activity activity)
        {
            this.Activity = activity;
        }
    }
}