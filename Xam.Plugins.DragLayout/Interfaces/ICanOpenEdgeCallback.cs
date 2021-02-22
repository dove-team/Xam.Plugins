namespace Xam.Plugins.DragLayout.Interfaces
{
    /// <summary>
    /// 是否可以打开边缘回调
    /// </summary>
    public interface ICanOpenEdgeCallback
    {
        /// <summary>
        /// 判断是否可以打开左边
        /// </summary>
        /// <param name="view">视图</param>
        /// <param name="xv">水平方向速度</param>
        /// <param name="yv">垂直方向速度</param>
        /// <returns>true，可以打开左边</returns>
        bool CanOpenLeft(DragLayout view, float xv, float yv);
        /// <summary>
        /// 判断是否可以打开右边
        /// </summary>
        /// <param name="view">视图</param>
        /// <param name="xv">水平方向速度</param>
        /// <param name="yv">垂直方向速度</param>
        /// <returns>true，可以打开右边</returns>
        bool CanOpenRight(DragLayout view, float xv, float yv);
        /// <summary>
        /// 判断是否可以打开上边
        /// </summary>
        /// <param name="view">视图</param>
        /// <param name="xv">水平方向速度</param>
        /// <param name="yv">垂直方向速度</param>
        /// <returns>true，可以打开上边</returns>
        bool CanOpenTop(DragLayout view, float xv, float yv);
        /// <summary>
        /// 判断是否可以打开下边
        /// </summary>
        /// <param name="view">视图</param>
        /// <param name="xv">水平方向速度</param>
        /// <param name="yv">垂直方向速度</param>
        /// <returns>true，可以打开下边</returns>
        bool CanOpenBottom(DragLayout view, float xv, float yv);
    }

}