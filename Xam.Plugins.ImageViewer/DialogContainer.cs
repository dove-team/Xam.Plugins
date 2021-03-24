using Android.App;
using Android.Views;

namespace Xam.Plugins.ImageViewer
{
    public class DialogContainer : ITargetContainer
    {
        private Dialog Dialog { get; }
        public DialogContainer(Dialog dialog)
        {
            this.Dialog = dialog;
        }
        public ViewGroup DecorView
        {
            get
            {
                return Dialog.Window != null ? (ViewGroup)Dialog.Window.DecorView : null;
            }
        }
    }
}