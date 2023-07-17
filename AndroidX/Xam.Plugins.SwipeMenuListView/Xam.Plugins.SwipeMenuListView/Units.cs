using Android.Content;
using Android.Util;

namespace Xam.Plugins.SwipeList
{
	public static class Units
	{
		public static int DpToPx(this Context context, int dp)
		{
			return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, context.Resources.DisplayMetrics);
		}
	}
}