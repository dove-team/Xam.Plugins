namespace Xam.Plugins.DanmakuMe
{
    public interface IPool<T>
    {
        T Get();
        void Release();
        int Count();
        void SetMaxSize(int max);
    }
}