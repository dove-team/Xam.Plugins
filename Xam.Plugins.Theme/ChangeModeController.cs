using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System.Collections.Generic;

namespace Xam.Plugins.Theme
{
    public class ChangeModeController
    {
        public const string ATTR_TEXTCOLOR = "textColorAttr";
        public const string ATTR_BACKGROUND = "backgroundAttr";
        public const string ATTR_BACKGROUND_DRAWABLE = "backgroundDrawableAttr";
        internal List<AttrEntity<View>> mBackGroundViews;
        internal List<AttrEntity<View>> mBackGroundDrawableViews;
        internal List<AttrEntity<TextView>> mTextColorViews;
        private static ChangeModeController mChangeModeController;
        private ChangeModeController() { }
        public static ChangeModeController Instance
        {
            get
            {
                if (mChangeModeController == null)
                    mChangeModeController = new ChangeModeController();
                return mChangeModeController;
            }
        }
        private void Init()
        {
            mBackGroundViews = new List<AttrEntity<View>>();
            mTextColorViews = new List<AttrEntity<TextView>>();
            mBackGroundDrawableViews = new List<AttrEntity<View>>();
        }
        public ChangeModeController Init(Activity activity, Class mClass)
        {
            Init();
            LayoutInflaterCompat.SetFactory2(LayoutInflater.From(activity), new LayoutInflaterFactory(this, mClass));
            return this;
        }
        public int GetAttr(Class draw, string attrName)
        {
            if (attrName == null || attrName.Trim().Equals("") || draw == null)
                return Resource.Color.colorPrimary;
            try
            {
                var field = draw.GetDeclaredField(attrName);
                return field.GetInt(attrName);
            }
            catch
            {
                return Resource.Color.colorPrimary;
            }
        }
        public void SetTheme(Context ctx, int Style_Day, int Style_Night)
        {
            if (ChangeModeHelper.GetChangeMode(ctx) == ChangeModeHelper.MODE_DAY)
            {
                ctx.SetTheme(Style_Day);
            }
            else if (ChangeModeHelper.GetChangeMode(ctx) == ChangeModeHelper.MODE_NIGHT)
            {
                ctx.SetTheme(Style_Night);
            }
        }
        public void ChangeNight(Activity ctx, int style)
        {
            if (mBackGroundDrawableViews == null || mTextColorViews == null || mBackGroundViews == null)
                throw new RuntimeException("请先调用init()初始化方法!");
            ChangeModeHelper.SetChangeMode(ctx, ChangeModeHelper.MODE_NIGHT);
            ctx.SetTheme(style);
            ShowAnimation(ctx);
            RefreshUI(ctx);
        }
        public void ChangeDay(Activity ctx, int style)
        {
            if (mBackGroundDrawableViews == null || mTextColorViews == null || mBackGroundViews == null)
                throw new RuntimeException("请先调用init()初始化方法!");
            ChangeModeHelper.SetChangeMode(ctx, ChangeModeHelper.MODE_DAY);
            ctx.SetTheme(style);
            ShowAnimation(ctx);
            RefreshUI(ctx);
        }
        private void RefreshUI(Activity ctx)
        {
            TypedValue typedValue = new TypedValue();
            var theme = ctx.Theme;
            theme.ResolveAttribute(Resource.Color.colorPrimary, typedValue, true);
            foreach (AttrEntity<View> entity in mBackGroundViews)
            {
                theme.ResolveAttribute(entity.ColorId, typedValue, true);
                entity.View.SetBackgroundResource(typedValue.ResourceId);
            }
            foreach (AttrEntity<View> entity in mBackGroundDrawableViews)
            {
                theme.ResolveAttribute(entity.ColorId, typedValue, true);
                entity.View.SetBackgroundResource(typedValue.ResourceId);
            }
            foreach (AttrEntity<TextView> entity in mTextColorViews)
            {
                theme.ResolveAttribute(entity.ColorId, typedValue, true);
                entity.View.SetTextColor(ctx.Resources.GetColor(typedValue.ResourceId));
            }
            RefreshStatusBar(ctx);
        }
        public TypedValue GetAttrTypedValue(Activity ctx, int attr)
        {
            TypedValue typedValue = new TypedValue();
            var theme = ctx.Theme;
            theme.ResolveAttribute(attr, typedValue, true);
            return typedValue;
        }
        private void RefreshStatusBar(Activity ctx)
        {
            if ((int)Build.VERSION.SdkInt >= 21)
            {
                TypedValue typedValue = new TypedValue();
                var theme = ctx.Theme;
                theme.ResolveAttribute(Resource.Color.colorPrimaryDark, typedValue, true);
                ctx.Window.SetStatusBarColor(ctx.Resources.GetColor(typedValue.ResourceId));
            }
        }
        private void ShowAnimation(Activity ctx)
        {
            View decorView = ctx.Window.DecorView;
            Bitmap cacheBitmap = GetCacheBitmapFromView(decorView);
            if (decorView is ViewGroup group && cacheBitmap != null)
            {
                View view = new View(ctx);
                view.SetBackgroundDrawable(new BitmapDrawable(ctx.Resources, cacheBitmap));
                ViewGroup.LayoutParams layoutParam = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                        ViewGroup.LayoutParams.MatchParent);
                group.AddView(view, layoutParam);
                ValueAnimator objectAnimator = ValueAnimator.OfFloat(1f, 0f);
                objectAnimator.SetDuration(500);
                objectAnimator.AddListener(new AnimatorListenerAdapter(decorView, view));
                objectAnimator.AddUpdateListener(new AnimatorUpdateListener(view));
                objectAnimator.Start();
            }
        }
        private Bitmap GetCacheBitmapFromView(View view)
        {
            bool drawingCacheEnabled = true;
            view.DrawingCacheEnabled = drawingCacheEnabled;
            view.BuildDrawingCache(drawingCacheEnabled);
            Bitmap drawingCache = view.DrawingCache;
            Bitmap bitmap;
            if (drawingCache != null)
            {
                bitmap = Bitmap.CreateBitmap(drawingCache);
                view.DrawingCacheEnabled = false;
            }
            else
            {
                bitmap = null;
            }
            return bitmap;
        }
        public void OnDestory()
        {
            mBackGroundViews.Clear();
            mTextColorViews.Clear();
            mBackGroundDrawableViews.Clear();
            mBackGroundViews = null;
            mTextColorViews = null;
            mBackGroundDrawableViews = null;
            mChangeModeController = null;
        }
        public ChangeModeController AddBackgroundColor(View view, int colorId)
        {
            mBackGroundViews.Add(new AttrEntity<View>(view, colorId));
            return this;
        }
        public ChangeModeController AddBackgroundDrawable(View view, int drawableId)
        {
            mBackGroundDrawableViews.Add(new AttrEntity<View>(view, drawableId));
            return this;
        }
        public ChangeModeController AddTextColor(View view, int colorId)
        {
            mTextColorViews.Add(new AttrEntity<TextView>((TextView)view, colorId));
            return this;
        }
    }
}