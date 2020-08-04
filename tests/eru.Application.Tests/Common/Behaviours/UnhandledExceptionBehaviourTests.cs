using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Behaviours;
using FluentAssertions;
using MELT;
using Microsoft.Extensions.Logging;
using Xunit;

namespace eru.Application.Tests.Common.Behaviours
{
    public class UnhandledExceptionBehaviourTests
    {
        [Fact]
        public async Task DoesUnhandledExceptionBehaviourWorksCorrectlyWhenNoExceptionIsThrown()
        {
            var loggerFactory = MELTBuilder.CreateLoggerFactory();
            var behaviour = new UnhandledExceptionBehaviour<SampleRequest, SampleResponse>(loggerFactory.CreateLogger<SampleRequest>());
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
        public Task DoesUnhandledExceptionBehaviourWorksCorrectlyWhenExceptionIsThrown()
        {
            var loggerFactory = MELTBuilder.CreateLoggerFactory();
            var behaviour = new UnhandledExceptionBehaviour<SampleRequest, SampleResponse>(loggerFactory.CreateLogger<SampleRequest>());
            var request = new SampleRequest
            {
                Version = "v2.0",
                IsWorking = true
            };

            Action a = () => behaviour.Handle(request, CancellationToken.None,
                () => throw new Exception("Test Exception")).GetAwaiter().GetResult();

            a.Should().Throw<Exception>();
            loggerFactory.LogEntries.Should().ContainSingle(x =>
                x.LogLevel == LogLevel.Error && x.Message ==
                "eru Request: Unhandled Exception for Request SampleRequest {Version = v2.0, IsWorking = True}");
            return Task.CompletedTask;
        }
    }
}