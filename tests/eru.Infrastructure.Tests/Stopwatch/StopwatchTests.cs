using FluentAssertions;
using Xunit;

namespace eru.Infrastructure.Tests.Stopwatch
{
    public class StopwatchTests
    {
        [Fact]
        public void DoesElapsedMillisecondsWorkCorrectly()
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            stopwatch.Stop();
            var implementation = new Infrastructure.Stopwatch.Stopwatch(stopwatch);

            implementation.ElapsedMilliseconds.Should().Be(stopwatch.ElapsedMilliseconds);
        }

        [Fact]
        public void DoesStopWorks()
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            stopwatch.Stop();
            var implementation = new Infrastructure.Stopwatch.Stopwatch(stopwatch);
            implementation.Stop();
            stopwatch.IsRunning.Should().BeFalse();
        }

        [Fact]
        public void DoesStartWorks()
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            var implementation = new Infrastructure.Stopwatch.Stopwatch(stopwatch);
            implementation.Start();
            stopwatch.IsRunning.Should().BeTrue();
        }
    }
}