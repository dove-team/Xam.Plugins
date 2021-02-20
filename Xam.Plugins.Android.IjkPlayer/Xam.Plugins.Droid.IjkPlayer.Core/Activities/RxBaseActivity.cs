using Android.OS;

namespace Xam.Plugins.Droid.IjkPlayer.Core.Activities
{
    public abstract class RxBaseActivity : RxAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //设置布局内容
            SetContentView(GetLayoutId());
            //初始化控件
            InitViews(savedInstanceState);
            //初始化ToolBar
            InitToolBar();
        }
        /// <summary>
        /// 设置布局layout
        /// </summary>
        /// <returns></returns>
        public abstract int GetLayoutId();
        /// <summary>
        /// 初始化views
        /// </summary>
        /// <param name="savedInstanceState"></param>
        public abstract void InitViews(Bundle savedInstanceState);
        /// <summary>
        /// 初始化toolbar
        /// </summary>
        public abstract void InitToolBar();
        /// <summary>
        /// 加载数据
        /// </summary>
        public virtual void LoadData() { }
        /// <summary>
        /// 显示进度条
        /// </summary>
        public void ShowProgressBar() { }
        /// <summary>
        /// 隐藏进度条
        /// </summary>
        public void HideProgressBar() { }
        /// <summary>
        /// 初始化recyclerView
        /// </summary>
        public void InitRecyclerView() { }
        /// <summary>
        /// 初始化refreshLayout
        /// </summary>
        public void InitRefreshLayout() { }
        /// <summary>
        /// 设置数据显示
        /// </summary>
        public void FinishTask() { }
    }
}