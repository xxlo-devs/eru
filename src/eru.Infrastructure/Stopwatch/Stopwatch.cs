using eru.Application.Common.Interfaces;

namespace eru.Infrastructure.Stopwatch
{
    public class Stopwatch : IStopwatch
    {
        private readonly System.Diagnostics.Stopwatch _stopwatch;

        public Stopwatch(System.Diagnostics.Stopwatch stopwatch)
        {
            _stopwatch = stopwatch;
        }

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;
    }
}