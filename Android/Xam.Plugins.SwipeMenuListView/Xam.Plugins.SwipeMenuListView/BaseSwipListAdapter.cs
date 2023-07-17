using Android.Widget;

namespace Xam.Plugins.SwipeList
{
    public abstract class BaseSwipListAdapter : BaseAdapter
    {
        public bool GetSwipEnableByPosition(int position) => true;
    }
}