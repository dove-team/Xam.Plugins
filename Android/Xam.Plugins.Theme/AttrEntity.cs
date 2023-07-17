namespace Xam.Plugins.Theme
{
    //https://github.com/zhangke3016/ChangeMode
    public sealed class AttrEntity<T>
    {
        public T View { get; }
        public int ColorId { get; }
        public AttrEntity(T view, int colorId)
        {
            this.View = view;
            this.ColorId = colorId;
        }
    }
}