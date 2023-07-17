using Android.Content;
using Android.Util;
using System;

namespace Xam.Plugins.DragLayout
{
    public class FlowLayout : DragLayout
    {
        /// <summary>
        /// 打开上、下面的速度
        /// </summary>
        private int openVy;
        /// <summary>
        /// 打开左、右面的速度
        /// </summary>
        private int openVx;
        public FlowLayout(Context context) : base(context)
        {
            Init();
        }
        public FlowLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }
        public FlowLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init();
        }
        public FlowLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init();
        }
        private void Init()
        {
            openVx = openVy = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, DEFAULT_SCROLL_VELOCITY_DP, Context.Resources.DisplayMetrics);
        }
        protected override bool CanOpenTop(float xv, float yv)
        {
            if (yv > 0)
                return Math.Abs(yv) >= openVy || LayerScrollY <= VerticalLayerScrollMin / 2;
            return base.CanOpenTop(xv, yv);
        }
        protected override bool CanOpenBottom(float xv, float yv)
        {
            if (yv < 0)
                return Math.Abs(yv) >= openVy || LayerScrollY >= VerticalLayerScrollMax / 2;
            return base.CanOpenBottom(xv, yv);
        }
        protected override bool CanOpenLeft(float xv, float yv)
        {
            if (xv > 0)
                return Math.Abs(xv) >= openVx || LayerScrollX <= HorizontalLayerScrollMin / 2;
            return base.CanOpenLeft(xv, yv);
        }
        protected override bool CanOpenRight(float xv, float yv)
        {
            if (xv < 0)
                return Math.Abs(xv) >= openVx || LayerScrollX >= HorizontalLayerScrollMax / 2;
            return base.CanOpenRight(xv, yv);
        }
    }
}