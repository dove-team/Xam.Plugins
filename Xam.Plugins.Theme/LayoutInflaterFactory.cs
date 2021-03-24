using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Xam.Plugins.Theme
{
    public class LayoutInflaterFactory : Object, LayoutInflater.IFactory2
    {
        private Class mClass { get; }
        private ChangeModeController Controller { get; }
        public LayoutInflaterFactory(ChangeModeController controller, Class mClass)
        {
            this.mClass = mClass;
            this.Controller = controller;
        }
        public View OnCreateView(View parent, string name, Context context, IAttributeSet attrs)
        {
            return OnDrawView(name, context, attrs);
        }
        public View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            return OnDrawView(name, context, attrs);
        }
        private View OnDrawView(string name, Context context, IAttributeSet attrs)
        {
            View view = null;
            try
            {
                if (name.IndexOf('.') == -1)
                {
                    if ("View".Equals(name))
                        view = LayoutInflater.From(context).CreateView(name, "android.view.", attrs);
                    if (view == null)
                        view = LayoutInflater.From(context).CreateView(name, "android.widget.", attrs);
                    if (view == null)
                        view = LayoutInflater.From(context).CreateView(name, "android.webkit.", attrs);
                }
                else
                {
                    if (view == null)
                        view = LayoutInflater.From(context).CreateView(name, null, attrs);
                }
                if (view != null)
                {
                    for (int i = 0; i < attrs.AttributeCount; i++)
                    {
                        if (attrs.GetAttributeName(i).Equals(ChangeModeController.ATTR_BACKGROUND))
                        {
                            Controller.mBackGroundViews.Add(new AttrEntity<View>(view, Controller.GetAttr(mClass, attrs.GetAttributeValue(i))));
                        }
                        if (attrs.GetAttributeName(i).Equals(ChangeModeController.ATTR_TEXTCOLOR))
                        {
                            Controller.mTextColorViews.Add(new AttrEntity<TextView>((TextView)view, Controller.GetAttr(mClass, attrs.GetAttributeValue(i))));
                        }
                        if (attrs.GetAttributeName(i).Equals(ChangeModeController.ATTR_BACKGROUND_DRAWABLE))
                        {
                            Controller.mBackGroundDrawableViews.Add(new AttrEntity<View>(view, Controller.GetAttr(mClass, attrs.GetAttributeValue(i))));
                        }
                    }
                }
            }
            catch { }
            return view;
        }
    }
}