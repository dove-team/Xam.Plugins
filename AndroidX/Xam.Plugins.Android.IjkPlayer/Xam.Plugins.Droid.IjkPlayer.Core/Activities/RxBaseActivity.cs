using Android.OS;

namespace Xam.Plugins.Droid.IjkPlayer.Core.Activities
{
    public abstract class RxBaseActivity : RxAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(GetLayoutId());
            InitViews(savedInstanceState);
            InitToolBar();
        }
        public abstract int GetLayoutId();
        public abstract void InitViews(Bundle savedInstanceState);
        public abstract void InitToolBar();
        public virtual void LoadData() { }
        public void ShowProgressBar() { }
        public void HideProgressBar() { }
        public void InitRecyclerView() { }
        public void InitRefreshLayout() { }
        public void FinishTask() { }
    }
}