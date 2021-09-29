using Android.Views;
using System;

namespace Xam.Plugins.SwipeList
{
    public sealed class SwipeOnGestureListener : GestureDetector.SimpleOnGestureListener
    {
        private SwipeMenuLayout Owner { get; }
        public SwipeOnGestureListener(SwipeMenuLayout owner)
        {
            Owner = owner;
        }
        public override bool OnDown(MotionEvent e)
        {
            Owner.IsFling = false;
            return true;
        }
        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            if (Math.Abs(e1.GetX() - e2.GetX()) > Owner.MIN_FLING && velocityX < Owner.MAX_VELOCITYX)
                Owner.IsFling = true;
            return base.OnFling(e1, e2, velocityX, velocityY);
        }
    }
}