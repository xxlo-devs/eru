using System.Threading.Tasks;
using eru.Application.Common.Behaviours;
using eru.Application.Common.Interfaces;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace eru.Application.Tests
{
    public class DependencyInjectionTests
    {
        [Fact]
        public Task AreAllBehaviourPipelinesAreRegisteredCorrectly()
        {
            var services = new ServiceCollection()
                .AddLogging()
                .AddApplication()
                .AddTransient(x=>new Mock<IStopwatch>().Object)
                .BuildServiceProvider();
                
                var pipelines = services.GetServices<IPipelineBehavior<SampleRequest, SampleResponse>>();

                pipelines.Should()
                    .ContainSingle(x=>x.GetType()==typeof(ValidationBehaviour<SampleRequest,SampleResponse>))
                    .And
                    .ContainSingle(x=>x.GetType()==typeof(PerformanceBehaviour<SampleRequest,SampleResponse>))
                    .And
                    .ContainSingle(x=>x.GetType()==typeof(UnhandledExceptionBehaviour<SampleRequest,SampleResponse>));
                return Task.CompletedTask;
        }
    }
}