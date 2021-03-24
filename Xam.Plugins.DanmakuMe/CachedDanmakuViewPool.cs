using System;

namespace Xam.Plugins.DanmakuMe
{
    public class CachedDanmakuViewPool : IPool<DanmakuView>
    {
        public int Count()
        {
            throw new NotImplementedException();
        }
        public DanmakuView Get()
        {
            throw new NotImplementedException();
        }
        public void Release()
        {
            throw new NotImplementedException();
        }
        public void SetMaxSize(int max)
        {
            throw new NotImplementedException();
        }
    }
}