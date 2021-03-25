using System.Collections.Generic;
using Xam.Plugins.DanmakuMe.Interface;

namespace Xam.Plugins.DanmakuMe.Listener
{
    public class ListenerInfo
    {
        public List<IOnEnterListener> OnEnterListeners;
        public List<IOnExitListener> OnExitListeners;
    }
}