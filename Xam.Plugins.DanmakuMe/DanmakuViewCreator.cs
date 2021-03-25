using System;
using Xam.Plugins.DanmakuMe.Interface;

namespace Xam.Plugins.DanmakuMe
{
    public class DanmakuViewCreator : IViewCreator<DanmakuView>
    {
        private Func<DanmakuView> Action { get; }
        public DanmakuViewCreator(Func<DanmakuView> action)
        {
            Action = action;
        }
        public DanmakuView Create()
        {
            return Action?.Invoke();
        }
    }
}