namespace eru.Application.Common.Interfaces
{
    public interface IStopwatch
    {
        void Start();
        void Stop();
        long ElapsedMilliseconds { get; }
    }
}