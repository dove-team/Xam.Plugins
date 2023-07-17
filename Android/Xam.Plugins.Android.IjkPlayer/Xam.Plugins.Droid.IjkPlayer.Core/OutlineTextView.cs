using System;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using APaint = Android.Graphics.Paint;

namespace Xam.Plugins.Droid.IjkPlayer.Core
{
    public class OutlineTextView : TextView
    {
        private TextPaint mTextPaint;
        private TextPaint mTextPaintOutline;
        private string mText = "";
        private float mBorderSize;
        private Color mBorderColor;
        private Color mColor;
        private float mSpacingMult = 1.0f;
        private float mSpacingAdd = 0;
        private bool mIncludePad = true;
        public OutlineTextView(Context context) : base(context)
        {
            InitPaint();
        }
        public OutlineTextView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitPaint();
        }
        public OutlineTextView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            InitPaint();
        }
        private void InitPaint()
        {
            mTextPaint = new TextPaint();
            mTextPaint.AntiAlias = true;
            mTextPaint.TextSize = TextSize;
            mTextPaint.Color = mColor;
            mTextPaint.SetStyle(APaint.Style.Fill);
            mTextPaint.SetTypeface(Typeface);
            mTextPaintOutline = new TextPaint();
            mTextPaintOutline.AntiAlias = true;
            mTextPaintOutline.TextSize = TextSize;
            mTextPaintOutline.Color = mBorderColor;
            mTextPaintOutline.SetStyle(APaint.Style.Stroke);
            mTextPaintOutline.SetTypeface(Typeface);
            mTextPaintOutline.StrokeWidth = mBorderSize;
        }
        public void SetText(string text)
        {
            base.SetText(text, BufferType.Normal);
            mText = text;
            RequestLayout();
            Invalidate();
        }
        public void SetTextSize(float size)
        {
            base.SetTextSize(ComplexUnitType.Px, size);
            RequestLayout();
            Invalidate();
            InitPaint();
        }
        public override void SetTextColor(Color color)
        {
            base.SetTextColor(color);
            mColor = color;
            Invalidate();
            InitPaint();
        }
        public override void SetShadowLayer(float radius, float dx, float dy, Color color)
        {
            base.SetShadowLayer(radius, dx, dy, color);
            mBorderSize = radius;
            mBorderColor = color;
            RequestLayout();
            Invalidate();
            InitPaint();
        }
        public void SetTypeface(Typeface tf, int style)
        {
            base.SetTypeface(tf, (TypefaceStyle)style);
            RequestLayout();
            Invalidate();
            InitPaint();
        }
        public void SetTypeface(Typeface tf)
        {
            base.SetTypeface(tf, TypefaceStyle.Normal);
            RequestLayout();
            Invalidate();
            InitPaint();
        }
        protected override void OnDraw(Canvas canvas)
        {
            Layout layout = new StaticLayout(Text, mTextPaintOutline, Width, Layout.Alignment.AlignCenter, mSpacingMult,
                    mSpacingAdd, mIncludePad);
            layout.Draw(canvas);
            layout = new StaticLayout(Text, mTextPaint, Width, Layout.Alignment.AlignCenter, mSpacingMult,
                    mSpacingAdd, mIncludePad);
            layout.Draw(canvas);
        }
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            Layout layout = new StaticLayout(Text, mTextPaintOutline,
                     MeasureWidth(widthMeasureSpec), Layout.Alignment.AlignCenter,
                     mSpacingMult, mSpacingAdd, mIncludePad);
            int ex = (int)(mBorderSize * 2 + 1);
            SetMeasuredDimension(MeasureWidth(widthMeasureSpec) + ex,
                    MeasureHeight(heightMeasureSpec) * layout.LineCount + ex);
        }
        private int MeasureWidth(int measureSpec)
        {
            int result;
            int specMode = Convert.ToInt32(MeasureSpec.GetMode(measureSpec));
            int specSize = MeasureSpec.GetSize(measureSpec);
            if (specMode == (int)MeasureSpecMode.Exactly)
                result = specSize;
            else
            {
                result = (int)mTextPaintOutline.MeasureText(mText) + PaddingLeft + PaddingRight;
                if (specMode == (int)MeasureSpecMode.AtMost)
                    result = Math.Min(result, specSize);
            }
            return result;
        }
        private int MeasureHeight(int measureSpec)
        {
            int specMode = Convert.ToInt32(MeasureSpec.GetMode(measureSpec));
            int specSize = MeasureSpec.GetSize(measureSpec);
            int mAscent = (int)mTextPaintOutline.Ascent();
            int result;
            if (specMode == (int)MeasureSpecMode.Exactly)
                result = specSize;
            else
            {
                result = (int)(-mAscent + mTextPaintOutline.Descent()) + PaddingTop + PaddingBottom;
                if (specMode == (int)MeasureSpecMode.Exactly)
                    result = Math.Min(result, specSize);
            }
            return result;
        }
    }
}