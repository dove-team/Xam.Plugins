using Android.Content;
using Android.Graphics;
using Android.Util;
using Java.Lang;
using Xam.Plugins.DragLayout.Interfaces;

namespace Xam.Plugins.DragLayout
{
    public class VerticalRefreshLayout : DragRefreshLayout
    {
        public ILoadCallback LoadCallback { get; set; }
        public IOnLoadStateChangedListener OnLoadStateChangedListener { get; set; }
        public VerticalRefreshLayout(Context context) : base(context)
        {
            Init();
        }
        public VerticalRefreshLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }
        public VerticalRefreshLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init();
        }
        public VerticalRefreshLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init();
        }
        private void Init()
        {
            SetOverScroll(OVER_SCROLL_TOP | OVER_SCROLL_BOTTOM);
            SetEdgeState(EDGE_STATE_DISABLE, EDGE_STATE_NORMAL, EDGE_STATE_DISABLE, EDGE_STATE_DISABLE);
        }
        protected override void OnEdgeStateChanged(Rect oldState, Rect state)
        {
            base.OnEdgeStateChanged(oldState, state);
            if (oldState.Top != state.Top)
            {
                if (null != OnLoadStateChangedListener)
                    OnLoadStateChangedListener.OnRefreshStateChanged(this, oldState.Top, state.Top);
                if (state.Top == EDGE_STATE_LOADING)
                {
                    OnRefresh();
                    if (null != LoadCallback)
                        LoadCallback.OnRefresh(this);
                }
            }
            if (oldState.Bottom != state.Bottom)
            {
                if (null != OnLoadStateChangedListener)
                    OnLoadStateChangedListener.OnLoadMoreStateChanged(this, oldState.Bottom, state.Bottom);
                if (state.Bottom == EDGE_STATE_LOADING)
                {
                    OnLoadMore();
                    if (null != LoadCallback)
                        LoadCallback.OnLoadMore(this);
                }
            }
        }
        public void PostRefresh(bool openEdge)
        {
            PostAfterLayout(new Runnable(() =>
            {
                DoRefresh(openEdge);
            }));
        }
        public void DoRefresh(bool openEdge)
        {
            Rect state = EdgeStateTag;
            if (state.Top != EDGE_STATE_LOADING)
            {
                state.Top = EDGE_STATE_LOADING;
                state.Bottom = EDGE_STATE_DISABLE;
                SetEdgeState(state);
                if (openEdge)
                    OpenTop(true);
            }
        }
        public void PostLoadMore(bool openEdge)
        {
            PostAfterLayout(new Runnable(() =>
            {
                DoLoadMore(openEdge);
            }));
        }
        public void DoLoadMore(bool openEdge)
        {
            Rect state = EdgeStateTag;
            if (state.Bottom != EDGE_STATE_LOADING)
            {
                state.Bottom = EDGE_STATE_LOADING;
                SetEdgeState(state);
                if (openEdge)
                    OpenBottom(true);
            }
        }
        protected void OnRefresh() { }
        protected void OnLoadMore() { }
        /// <summary>
        /// 完成读取
        /// </summary>
        /// <param name="hasMore">是否有更多数据</param>
        public void FinishLoad(bool hasMore)
        {
            Rect state = EdgeStateTag;
            if (state.Top == EDGE_STATE_LOADING)
            {
                if (LayerScrollY == 0)
                    state.Top = EDGE_STATE_NORMAL;
                else
                    state.Top = EDGE_STATE_FINISH;
            }
            if (hasMore)
            {
                if (LayerScrollY == 0)
                    state.Bottom = EDGE_STATE_NORMAL;
                else
                    state.Bottom = EDGE_STATE_FINISH;
            }
            else
                state.Bottom = EDGE_STATE_NONE;
            SetEdgeState(state);
        }
        public int RefreshState
        {
            get
            {
                return EdgeStateTag.Top;
            }
        }
        public int LoadMoreState
        {
            get
            {
                return EdgeStateTag.Bottom;
            }
        }
        public bool IsRefreshing
        {
            get
            {
                return RefreshState == EDGE_STATE_LOADING;
            }
        }
        public bool IsLoadingMore
        {
            get
            {
                return LoadMoreState == EDGE_STATE_LOADING;
            }
        }
    }
}