namespace Xam.Plugins.DanmakuMe
{
    public interface IPool<T>
    {
        T Get();
        void Release();
        int Count();
        int MaxSize { get; set; }
    }
}