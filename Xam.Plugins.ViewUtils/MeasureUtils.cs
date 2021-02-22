using Android.Graphics;
using Android.Views;
using Java.Lang;

namespace Xam.Plugins.ViewUtils
{
    public sealed class MeasureUtils
    {
        /// <summary>
        /// 制作属于自己的测量规格，用于测量子视图在{@link ViewGroup#onMeasure(int, int) ViewGroup.onMeasure}
        ///中使用，得出一个自身测量规格，然后使用{@link #makeChildMeasureSpec(int, int, int) makeChildMeasureSpec}
        ///   测量子视图规格
        /// </summary>
        /// <param name="parentMeasureSpec">父测量规格</param>
        /// <param name="padding">内间距</param>
        /// <returns>自身测量规格</returns>
        public static int MakeSelfMeasureSpec(int parentMeasureSpec, int padding)
        {
            var mode = View.MeasureSpec.GetMode(parentMeasureSpec);
            int size = View.MeasureSpec.GetSize(parentMeasureSpec);
            size = Math.Max(0, size - padding);
            return View.MeasureSpec.MakeMeasureSpec(size, mode);
        }
        /// <summary>
        /// 制作子测量规格
        /// </summary>
        /// <param name="parentMeasureSpec">父测量规格</param>
        /// <param name="layoutParamSize">LayoutParams的大小</param>
        /// <param name="margin">外边距</param>
        /// <returns>子的测量规格</returns>
        public static int MakeChildMeasureSpec(int parentMeasureSpec, int layoutParamSize, int margin)
        {
            int size = View.MeasureSpec.GetSize(parentMeasureSpec);
            size = Math.Max(size - margin, 0);
            if (layoutParamSize == ViewGroup.LayoutParams.MatchParent)
                return View.MeasureSpec.MakeMeasureSpec(size, MeasureSpecMode.Exactly);
            else if (layoutParamSize == ViewGroup.LayoutParams.WrapContent)
            {
                var mode = View.MeasureSpec.GetMode(parentMeasureSpec);
                if (mode == MeasureSpecMode.Exactly || mode == MeasureSpecMode.AtMost)
                    return View.MeasureSpec.MakeMeasureSpec(size, MeasureSpecMode.AtMost);
                else
                    return View.MeasureSpec.MakeMeasureSpec(size, MeasureSpecMode.Unspecified);
            }
            else
                return View.MeasureSpec.MakeMeasureSpec(layoutParamSize, MeasureSpecMode.Exactly);
        }
        /// <summary>
        /// 测量子视图
        /// </summary>
        /// <param name="child">子视图</param>
        /// <param name="parentWidthMeasureSpec">父视图宽测量规格</param>
        /// <param name="parentHeightMeasureSpec">父视图高测量规格</param>
        public static void MeasureChild(View child, int parentWidthMeasureSpec, int parentHeightMeasureSpec)
        {
            int horizontalMargin = 0, verticalMargin = 0;
            ViewGroup.LayoutParams lp = child.LayoutParameters;
            if (lp is ViewGroup.MarginLayoutParams @params)
            {
                horizontalMargin = @params.LeftMargin + @params.RightMargin;
                verticalMargin = @params.TopMargin + @params.BottomMargin;
            }
            int childWidthMeasureSpec = MakeChildMeasureSpec(parentWidthMeasureSpec, lp.Width, horizontalMargin);
            int childHeightMeasureSpec = MakeChildMeasureSpec(parentHeightMeasureSpec, lp.Height, verticalMargin);
            child.Measure(childWidthMeasureSpec, childHeightMeasureSpec);
        }
        /// <summary>
        /// 获取测量的尺寸
        /// </summary>
        /// <param name="contentSize">内容大小（保护padding）</param>
        /// <param name="parentMeasureSpec">父视图测量规格</param>
        /// <returns>测量的尺寸，用于{@link ViewGroup#setMeasuredDimension(int, int) ViewGroup.setMeasuredDimension}</returns>
        public static int GetMeasuredDimension(int contentSize, int parentMeasureSpec)
        {
            var mode = View.MeasureSpec.GetMode(parentMeasureSpec);
            int size = View.MeasureSpec.GetSize(parentMeasureSpec);
            if (mode == MeasureSpecMode.AtMost)
                return Math.Min(size, contentSize);
            else if (mode == MeasureSpecMode.Exactly)
                return size;
            else
                return contentSize;
        }
        /// <summary>
        /// 计算视图占用的宽度
        /// </summary>
        /// <param name="view">视图</param>
        /// <returns>视图占用的宽度</returns>
        public static int GetViewWidthSpace(View view)
        {
            int width = view.MeasuredWidth;
            ViewGroup.LayoutParams lp = view.LayoutParameters;
            if (lp is ViewGroup.MarginLayoutParams @params)
                width += @params.LeftMargin + @params.RightMargin;
            return width;
        }
        /// <summary>
        /// 计算视图占用的高度
        /// </summary>
        /// <param name="view">视图占用的高度</param>
        /// <returns>视图占用的高度</returns>
        public static int GetViewHeightSpace(View view)
        {
            int height = view.MeasuredHeight;
            ViewGroup.LayoutParams lp = view.LayoutParameters;
            if (lp is ViewGroup.MarginLayoutParams @params)
                height += @params.TopMargin + @params.BottomMargin;
            return height;
        }
        /// <summary>
        /// 计算布局所需的值
        /// </summary>
        /// <param name="view">视图</param>
        /// <param name="layoutX">布局的位置X</param>
        /// <param name="layoutY">布局的位置Y</param>
        /// <param name="layoutOut">返回布局的位置</param>
        /// <param name="spaceOut">返回占用的位置</param>
        public static void ComputeLayout(View view, int layoutX, int layoutY, Rect layoutOut, Rect spaceOut)
        {
            ViewGroup.LayoutParams lp = view.LayoutParameters;
            spaceOut.Left = layoutX;
            spaceOut.Top = layoutY;
            if (lp is ViewGroup.MarginLayoutParams @params)
            {
                layoutOut.Left = layoutX + @params.LeftMargin;
                layoutOut.Top = layoutY + @params.TopMargin;
                layoutOut.Right = layoutOut.Left + view.MeasuredWidth;
                layoutOut.Bottom = layoutOut.Top + view.MeasuredHeight;
                spaceOut.Right = layoutOut.Right + @params.RightMargin;
                spaceOut.Bottom = layoutOut.Bottom + @params.BottomMargin;
            }
            else
            {
                layoutOut.Left = layoutX;
                layoutOut.Top = layoutY;
                layoutOut.Right = layoutOut.Left + view.MeasuredWidth;
                layoutOut.Bottom = layoutOut.Top + view.MeasuredHeight;
                spaceOut.Right = layoutOut.Right;
                spaceOut.Bottom = layoutOut.Bottom;
            }
        }
    }
}