using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Application.Common.Exceptions;
using eru.Application.Configuration;
using eru.Application.Configuration.Database;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace eru.Application.Tests.Configuration
{
    public class ConfigExtensionsTests
    {
        [Fact]
        public void DoesLoadConfigsWorkCorrectly()
        {
            IServiceCollection services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new []
                {
                    new KeyValuePair<string, string>("DatabaseConfig:Server", "localhost"),
                    new KeyValuePair<string, string>("DatabaseConfig:Database", "TestDatabase"),
                    new KeyValuePair<string, string>("DatabaseConfig:Port", "1234"),
                    new KeyValuePair<string, string>("DatabaseConfig:Username", "TestUser"),
                    new KeyValuePair<string, string>("DatabaseConfig:Password", "TestPassword"),
                    new KeyValuePair<string, string>("DatabaseConfig:Pooling", "false"),
                    new KeyValuePair<string, string>("DatabaseConfig:MinPoolSize", "10"),
                    new KeyValuePair<string, string>("DatabaseConfig:MaxPoolSize", "11"),
                    new KeyValuePair<string, string>("DatabaseConfig:ConnectionLifetime", "1000"),
                })
                .Build();

            services.LoadConfigs(configuration);
            var serviceProvider = services.BuildServiceProvider();
            
            var classesDatabaseConfig = serviceProvider.GetService<DatabaseConfig>();
            classesDatabaseConfig.Should().NotBeNull().And.BeEquivalentTo(new DatabaseConfig()
                {
                    Server = "localhost",
                    ConnectionLifetime = 1000,
                    Database = "TestDatabase",
                    MaxPoolSize = 11,
                    MinPoolSize = 10,
                    Password = "TestPassword",
                    Pooling = false,
                    Port = 1234,
                    Username = "TestUser"
                });
        }

        [Fact]
        public void ThrowsErrorOnConfigurationWithoutRequiredConfig()
        {
            IServiceCollection services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .Build();

            Action action = () => services.LoadConfigs(configuration);

            action.Should().Throw<RequiredConfigNotPresentException>();
        }

        [Fact]
        public void ThrowsValidationExceptionOnNotValidParameter()
        {
            IServiceCollection services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new []
                {
                    new KeyValuePair<string, string>("DatabaseConfig:Server", "localhost"),
                    new KeyValuePair<string, string>("DatabaseConfig:Database", "TestDatabase"),
                    new KeyValuePair<string, string>("DatabaseConfig:Port", "1234"),
                    new KeyValuePair<string, string>("DatabaseConfig:Password", "TestPassword"),
                    new KeyValuePair<string, string>("DatabaseConfig:Pooling", "false"),
                    new KeyValuePair<string, string>("DatabaseConfig:MinPoolSize", "10"),
                    new KeyValuePair<string, string>("DatabaseConfig:MaxPoolSize", "11"),
                    new KeyValuePair<string, string>("DatabaseConfig:ConnectionLifetime", "1000"),
                })
                .Build();
            
            Action action = () => services.LoadConfigs(configuration);

            action.Should().Throw<ValidationException>();
        }
    }
}