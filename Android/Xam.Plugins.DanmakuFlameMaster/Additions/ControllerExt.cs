using JavaObject = Java.Lang.Object;

namespace Master.Flame.Danmaku.Controller
{
    public partial class DanmakuFilters : JavaObject
    {
        public abstract partial class BaseDanmakuFilter : JavaObject, IDanmakuFilter
        {
            public virtual void SetData(JavaObject p0) { }
        }
    }
}