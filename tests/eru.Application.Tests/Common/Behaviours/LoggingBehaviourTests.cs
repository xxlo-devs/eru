using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Behaviours;
using FluentAssertions;
using MELT;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eru.Application.Tests.Common.Behaviours
{
    public class LoggingBehaviourTests
    {
        [Fact]
        public async Task DoesLoggingBehaviourWorksCorrectly()
        {
            var loggerFactory = MELTBuilder.CreateLoggerFactory();
            var loggingBehaviour = new LoggingBehaviour<SampleRequest>(loggerFactory.CreateLogger<SampleRequest>());
            var request = new SampleRequest
            {
                Version = "v2.0",
                IsWorking = true
            };

            await loggingBehaviour.Process(request, CancellationToken.None);
            
            loggerFactory.LogEntries.Should()
                .ContainSingle(x => x.LogLevel == LogLevel.Information &&  x.Message == "eru Request: SampleRequest {Version = v2.0, IsWorking = True}");
        }
    }
}