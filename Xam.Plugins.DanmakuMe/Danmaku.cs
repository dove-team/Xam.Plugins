namespace Xam.Plugins.DanmakuMe
{
    public sealed class Danmaku
    {
        public const string COLOR_WHITE = "#ffffffff";
        public const string COLOR_RED = "#ffff0000";
        public const string COLOR_GREEN = "#ff00ff00";
        public const string COLOR_BLUE = "#ff0000ff";
        public const string COLOR_YELLOW = "#ffffff00";
        public const string COLOR_PURPLE = "#ffff00ff";
        public const int DEFAULT_TEXT_SIZE = 24;
        public string Text;
        public string Color = COLOR_WHITE;
        public int Size = DEFAULT_TEXT_SIZE;
        public DanmaukuMode Mode = DanmaukuMode.Scroll;
        public enum DanmaukuMode
        {
            Scroll,
            Top,
            Bottom
        }
        public Danmaku() { }
        public Danmaku(string text, int textSize, DanmaukuMode mode, string color)
        {
            this.Text = text;
            this.Size = textSize;
            this.Mode = mode;
            this.Color = color;
        }
        public override string ToString()
        {
            return string.Concat("Danmaku{",
                    "text='", Text, '\'',
                    ", textSize=", Size,
                    ", mode=", Mode,
                    ", color='", Color, '\'', '}');
        }
    }
}