using Android.Content;

namespace Xam.Plugins.Theme
{
    public sealed class ChangeModeHelper
    {
        public const int MODE_DAY = 1;
        public const int MODE_NIGHT = 2;
        private const string Mode = "mode";
        public static void SetChangeMode(Context ctx, int mode)
        {
            var sp = ctx.GetSharedPreferences("config_mode", FileCreationMode.Private);
            sp.Edit().PutInt(Mode, mode).Commit();
        }
        public static int GetChangeMode(Context ctx)
        {
            var sp = ctx.GetSharedPreferences("config_mode", FileCreationMode.Private);
            return sp.GetInt(Mode, MODE_DAY);
        }
    }
}