using Android.Widget;
using System;
using System.Collections.Generic;

namespace Xam.Plugins.DanmakuMe
{
    public static class Ext
    {
        public static FrameLayout Get(this WeakReference reference)
        {
            try
            {
                return reference.Target as FrameLayout;
            }
            catch { }
            return default;
        }
        public static bool Offer<T>(this LinkedList<T> list, T obj)
        {
            try
            {
                list.AddFirst(obj);
                return true;
            }
            catch { }
            return false;
        }
        public static T Poll<T>(this LinkedList<T> list)
        {
            LinkedListNode<T> obj = default;
            try
            {
                obj = list.First;
                return obj.Value;
            }
            catch { }
            finally
            {
                list.Remove(obj);
            }
            return default;
        }
    }
}