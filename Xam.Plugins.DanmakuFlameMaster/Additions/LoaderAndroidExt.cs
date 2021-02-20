using JavaObject = Java.Lang.Object;
using Master.Flame.Danmaku.Danmaku.Parser;

namespace Master.Flame.Danmaku.Danmaku.Loader.Android
{
	public partial class AcFunDanmakuLoader : JavaObject, ILoader
	{
		IDataSource ILoader.DataSource { get; }
	}
	public partial class BiliDanmakuLoader : JavaObject, ILoader
	{
		IDataSource ILoader.DataSource { get; }
	}
}