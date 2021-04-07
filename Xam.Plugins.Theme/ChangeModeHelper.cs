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
            try
            {
                var sp = ctx.GetSharedPreferences("config_mode", FileCreationMode.Private);
                if (sp != null)
                    sp.Edit().PutInt(Mode, mode).Commit();
            }
            catch { }
        }
        public static int GetChangeMode(Context ctx)
        {
            try
            {
                var sp = ctx.GetSharedPreferences("config_mode", FileCreationMode.Private);
                return sp == null ? default : sp.GetInt(Mode, MODE_DAY);
            }
            catch { }
            return default;
        }
    }
}