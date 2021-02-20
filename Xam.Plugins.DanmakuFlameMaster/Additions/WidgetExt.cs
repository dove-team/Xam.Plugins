using Android.Views;

namespace Master.Flame.Danmaku.UI.Widget
{
	public partial class DanmakuSurfaceView : SurfaceView, ISurfaceHolderCallback, Controller.IDanmakuView, Controller.IDanmakuViewController
	{
		public void SetVisibility(int p0) { }
	}
	public partial class DanmakuTextureView : TextureView, TextureView.ISurfaceTextureListener, Controller.IDanmakuView, Controller.IDanmakuViewController
	{
		public void SetVisibility(int p0) { }
	}
	public partial class DanmakuView : View, Controller.IDanmakuView, Controller.IDanmakuViewController
	{
		public void SetVisibility(int p0) { }
	}
}