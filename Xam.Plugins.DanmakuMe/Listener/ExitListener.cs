using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xam.Plugins.DanmakuMe.Interface;

namespace Xam.Plugins.DanmakuMe.Listener
{
    public class ExitListener : IOnExitListener
    {
        private Action Action { get; }
        public ExitListener(Action action)
        {
            Action = action;
        }
        public void OnExit(DanmakuView view)
        {
            Action?.Invoke();
        }
    }
}