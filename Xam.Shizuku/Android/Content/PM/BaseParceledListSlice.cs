using Java.Lang;
using System.Collections.Generic;

namespace Android.Content.PM
{
    public abstract class BaseParceledListSlice<T>
    {
        public virtual List<T> GetList()
        {
            throw new RuntimeException("STUB");
        }
    }
}