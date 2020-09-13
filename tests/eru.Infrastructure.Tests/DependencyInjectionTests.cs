using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Exceptions;
using eru.Application.Common.Interfaces;
using eru.Infrastructure.Persistence;
using FluentAssertions;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Xunit;

namespace eru.Infrastructure.Tests
{
    public class DependencyInjectionTests
    {
        [Fact]
        public Task IsAnyIStopwatchImplementationAvailable()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new []
                {
                    new KeyValuePair<string, string>("Database:Type", "unit-testing"), 
                })
                .Build();
            var serviceProvider = new ServiceCollection()
                .AddInfrastructure(configuration)
                .BuildServiceProvider();

            var stopwatch = serviceProvider.GetService<IStopwatch>();
            stopwatch.Should().NotBeNull();
            return Task.CompletedTask;
        }
    }
}