using Android.App;
using Android.Views;
using Android.Views.Animations;
using Java.Lang;
using DialogFragment = Android.Support.V4.App.DialogFragment;

namespace Xam.Plugins.ImageViewer
{
    public sealed class Viewer
    {
        public static ViewerConfig DefaultConfig { get; set; } = new ViewerConfig();
        private Viewer() { }
        public static void Unregister(View view)
        {
            view.SetOnTouchListener(null);
        }
        public sealed class Builder
        {
            private bool mDisposed = false;
            private View TargetView;
            private ViewerConfig Config;
            public ITargetContainer TargetContainer;
            private IZoomListener zoomListener;
            public IZoomListener ZoomListener
            {
                get
                {
                    return zoomListener;
                }
                set
                {
                    CheckNotDisposed();
                    zoomListener = value;
                }
            }
            private IInterpolator zoomInterpolator;
            public IInterpolator ZoomInterpolator
            {
                get { return zoomInterpolator; }
                set
                {
                    CheckNotDisposed();
                    zoomInterpolator = value;
                }
            }
            private ITapListener tapListener;
            public ITapListener TapListener
            {
                get
                {
                    return tapListener;
                }
                set
                {
                    CheckNotDisposed();
                    tapListener = value;
                }
            }
            private ILongPressListener longPressListener;
            public ILongPressListener LongPressListener
            {
                get
                {
                    return longPressListener;
                }
                set
                {
                    CheckNotDisposed();
                    longPressListener = value;
                }
            }
            private IDoubleTapListener doubleTapListener;
            public IDoubleTapListener DoubleTapListener
            {
                get
                {
                    return doubleTapListener;
                }
                set
                {
                    CheckNotDisposed();
                    doubleTapListener = value;
                }
            }
            public Builder(Activity activity)
            {
                this.TargetContainer = new ActivityContainer(activity);
            }
            public Builder(Dialog dialog)
            {
                this.TargetContainer = new DialogContainer(dialog);
            }
            public Builder(DialogFragment dialogFragment)
            {
                this.TargetContainer = new DialogFragmentContainer(dialogFragment);
            }
            public Builder Target(View target)
            {
                this.TargetView = target;
                return this;
            }
            public Builder AnimateZooming(bool animate)
            {
                CheckNotDisposed();
                if (Config == null) Config = new ViewerConfig();
                this.Config.ZoomAnimationEnabled = animate;
                return this;
            }
            public Builder EnableImmersiveMode(bool enable)
            {
                CheckNotDisposed();
                if (Config == null) Config = new ViewerConfig();
                this.Config.ImmersiveModeEnabled = enable;
                return this;
            }
            public Builder Interpolator(IInterpolator interpolator)
            {
                CheckNotDisposed();
                this.ZoomInterpolator = interpolator;
                return this;
            }
            public void Register()
            {
                CheckNotDisposed();
                if (Config == null)
                    Config = DefaultConfig;
                if (TargetContainer == null)
                    throw new IllegalArgumentException("Target container must not be null");
                if (TargetView == null)
                    throw new IllegalArgumentException("Target view must not be null");
                TargetView.SetOnTouchListener(new ZoomableTouchListener(TargetContainer, TargetView,
                        Config, ZoomInterpolator, ZoomListener, TapListener, LongPressListener,
                        DoubleTapListener));
                mDisposed = true;
            }
            private void CheckNotDisposed()
            {
                if (mDisposed) throw new IllegalStateException("Builder already disposed");
            }
        }
    }
}