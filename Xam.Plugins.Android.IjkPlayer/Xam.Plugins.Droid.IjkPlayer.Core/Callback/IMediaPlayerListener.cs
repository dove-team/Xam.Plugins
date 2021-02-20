namespace Xam.Plugins.Droid.IjkPlayer.Core.Callback
{

    public interface IMediaPlayerListener
    {
        void Start();
        void Pause();
        int GetDuration();
        int GetCurrentPosition();
        void SeekTo(long pos);
        bool IsPlaying();
        int GetBufferPercentage();
        bool CanPause();
    }
}