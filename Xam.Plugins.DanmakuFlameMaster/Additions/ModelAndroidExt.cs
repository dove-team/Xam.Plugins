using JavaObject = Java.Lang.Object;

namespace Master.Flame.Danmaku.Danmaku.Model.Android
{
	public partial class AndroidDisplayer : AbsDisplayer
	{
		protected override JavaObject RawExtraData { get; set; }
		public override void DrawDanmaku(BaseDanmaku p0, JavaObject p1, float p2, float p3, bool p4) { }
		public override void SetTypeFace(JavaObject p0) { }
	}
	public partial class DanmakuFactory : JavaObject
	{
		public unsafe void CustomFillLinePathData(BaseDanmaku item, float[][] points, float scaleX, float scaleY)
		{
			FillLinePathData(item, points, scaleX, scaleY);
		}
	}
}