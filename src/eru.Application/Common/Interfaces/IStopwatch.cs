namespace eru.Application.Common.Interfaces
{
    public interface IStopwatch
    {
        void Start();
        void Stop();
        int ElapsedMilliseconds { get; }
    }
}