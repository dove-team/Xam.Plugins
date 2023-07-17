using DialogFragment = Android.Support.V4.App.DialogFragment;

namespace Xam.Plugins.ImageViewer
{
    public class DialogFragmentContainer : DialogContainer
    {
       public DialogFragmentContainer(DialogFragment dialog) : base(dialog.Dialog) { }
    }
}