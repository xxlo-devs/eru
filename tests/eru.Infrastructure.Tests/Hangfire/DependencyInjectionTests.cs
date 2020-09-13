using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Xunit;

namespace eru.Infrastructure.Tests.Hangfire
{
    public class DependencyInjectionTests
    {
        [Fact]
        public Task IsHangfireCorrectlyConfigured()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                    {
                        new KeyValuePair<string, string>("Database:Type", "unit-testing"),
                    })
                .Build();
            var serilog = new LoggerConfiguration().CreateLogger();
            var serviceProvider = new ServiceCollection()
                .AddSingleton(serilog)
                .AddInfrastructure(configuration)
                .BuildServiceProvider();

            var hangfireConfigurations = serviceProvider.GetService<IGlobalConfiguration>();
            hangfireConfigurations.Should().NotBeNull();
            return Task.CompletedTask;
        }
    }
}