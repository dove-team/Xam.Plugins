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
        public string text;
        public int size = DEFAULT_TEXT_SIZE;
        public Mode mode = Mode.scroll;
        public string color = COLOR_WHITE;
        public enum Mode
        {
            scroll, top, bottom
        }
        public Danmaku() { }
        public Danmaku(string text, int textSize, Mode mode, string color)
        {
            this.text = text;
            this.size = textSize;
            this.mode = mode;
            this.color = color;
        }
        public override string ToString()
        {
            return "Danmaku{" +
                    "text='" + text + '\'' +
                    ", textSize=" + size +
                    ", mode=" + mode +
                    ", color='" + color + '\'' +
                    '}';
        }
    }
}