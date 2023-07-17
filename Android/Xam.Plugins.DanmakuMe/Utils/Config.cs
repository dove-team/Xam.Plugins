namespace Xam.Plugins.DanmakuMe.Utils
{
    public class Config
    {
        /// <summary>
        /// 行高，单位px
        /// </summary>
        public int LineHeight { get; set; }
        /// <summary>
        /// 滚动弹幕显示时长
        /// </summary>
        public int durationScroll;
        public int DurationScroll
        {
            get
            {
                if (durationScroll == 0)
                    durationScroll = 10000;
                return durationScroll;
            }
            set
            {
                durationScroll = value;
            }
        }
        /// <summary>
        /// 顶部弹幕显示时长
        /// </summary>
        private int durationTop;
        public int DurationTop
        {
            get
            {
                if (durationTop == 0)
                    durationTop = 5000;
                return durationTop;
            }
            set
            {
                durationTop = value;
            }
        }
        /// <summary>
        /// 底部弹幕的显示时长
        /// </summary>
        private int durationBottom;
        public int DurationBottom
        {
            get
            {
                if (durationBottom == 0)
                    durationBottom = 5000;
                return durationBottom;
            }
            set
            {
                durationBottom = value;
            }
        }
        /// <summary>
        /// 滚动弹幕的最大行数
        /// </summary>
        public int maxScrollLine;
        public int GetMaxScrollLine()
        {
            return maxScrollLine;
        }
        public int GetMaxDanmakuLine()
        {
            if (maxScrollLine == 0)
                maxScrollLine = 12;
            return maxScrollLine;
        }
        public void SetMaxScrollLine(int maxScrollLine)
        {
            this.maxScrollLine = maxScrollLine;
        }
    }
}