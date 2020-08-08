using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Behaviours;
using eru.Application.Common.Interfaces;
using FluentAssertions;
using MELT;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eru.Application.Tests.Common.Behaviours
{
    public class PerformanceBehaviourTests
    {
        [Fact]
        public async Task DoesPerformanceBehaviourWorksCorrectlyWhenRequestIsFast()
        {
            var fakeStopwatch = new Mock<IStopwatch>();
            fakeStopwatch.Setup(x => x.ElapsedMilliseconds).Returns(100);
            var loggerFactory = MELTBuilder.CreateLoggerFactory();
            var behaviour = new PerformanceBehaviour<SampleRequest, SampleResponse>(fakeStopwatch.Object, 
                loggerFactory.CreateLogger<SampleRequest>());
            var request = new SampleRequest
            {
                Version = "v2.0",
                IsWorking = true
            };

            await behaviour.Handle(request, CancellationToken.None,
                () => Task.FromResult(new SampleResponse {HasWorked = true}));

            loggerFactory.LogEntries.Should().BeEmpty();
        }

        [Fact]
        public async Task DoesPerformanceBehaviourWorksCorrectlyWhenRequestIsSlow()
        {
            var fakeStopwatch = new Mock<IStopwatch>();
            fakeStopwatch.Setup(x => x.ElapsedMilliseconds).Returns(1000);
            var loggerFactory = MELTBuilder.CreateLoggerFactory();
            var behaviour = new PerformanceBehaviour<SampleRequest, SampleResponse>(fakeStopwatch.Object, 
                loggerFactory.CreateLogger<SampleRequest>());
            var request = new SampleRequest
            {
                Version = "v2.0",
                IsWorking = true
            };

            await behaviour.Handle(request, CancellationToken.None,
                () => Task.FromResult(new SampleResponse {HasWorked = true}));

            loggerFactory.LogEntries.Should().ContainSingle(x=>x.LogLevel == LogLevel.Warning && x.Message == "eru Long Running Request: SampleRequest (1000 milliseconds) {Version = v2.0, IsWorking = True}");
        }
    }
}