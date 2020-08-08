using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Exceptions;
using eru.Application.Common.Interfaces;
using eru.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace eru.Infrastructure.Tests
{
    public class DependencyInjectionTests
    {
        [Fact]
        public async Task CanSqliteBeSelectedWithCorrectConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new []
                {
                    new KeyValuePair<string, string>("Database:Type", "sqlite"), 
                    new KeyValuePair<string, string>("Database:ConnectionString", "Data Source=eru.db"), 
                })
                .Build();
            var serviceProvider = new ServiceCollection()
                .AddScoped<IConfiguration>(_ => configuration)
                .AddInfrastructure(configuration)
                .BuildServiceProvider();
            
            serviceProvider.GetService<IApplicationDbContext>().Should().NotBeNull();
            
            var dbContext = serviceProvider.GetService<ApplicationDbContext>();
            dbContext.Should().NotBeNull();
            
            await dbContext.Database.EnsureCreatedAsync();
            (await serviceProvider.GetService<ApplicationDbContext>().Database.CanConnectAsync()).Should().BeTrue();
            await dbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public Task ExceptionIsThrownUponNoSqliteConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new []
                {
                    new KeyValuePair<string, string>("Database:Type", "sqlite"),
                })
                .Build();
            Action serviceProviderCreation = () => new ServiceCollection()
                .AddScoped<IConfiguration>(_ => configuration)
                .AddInfrastructure(configuration)
                .BuildServiceProvider();

            serviceProviderCreation.Should().Throw<DatabaseSettingsException>();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task CanInMemoryDatabaseBeSelected()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new []
                {
                    new KeyValuePair<string, string>("Database:Type", "inmemory"),
                })
                .Build();
            var serviceProvider = new ServiceCollection()
                .AddScoped<IConfiguration>(_ => configuration)
                .AddInfrastructure(configuration)
                .BuildServiceProvider();
            
            serviceProvider.GetService<IApplicationDbContext>().Should().NotBeNull();
            
            var dbContext = serviceProvider.GetService<ApplicationDbContext>();
            dbContext.Should().NotBeNull();
            
            await dbContext.Database.EnsureCreatedAsync();
            (await serviceProvider.GetService<ApplicationDbContext>().Database.CanConnectAsync()).Should().BeTrue();
            await dbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task InMemoryDatabaseIsUsedWhenNoDatabaseConfigIsAvaiable()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var serviceProvider = new ServiceCollection()
                .AddScoped<IConfiguration>(_ => configuration)
                .AddInfrastructure(configuration)
                .BuildServiceProvider();
            
            serviceProvider.GetService<IApplicationDbContext>().Should().NotBeNull();
            
            var dbContext = serviceProvider.GetService<ApplicationDbContext>();
            dbContext.Should().NotBeNull();
            
            await dbContext.Database.EnsureCreatedAsync();
            (await serviceProvider.GetService<ApplicationDbContext>().Database.CanConnectAsync()).Should().BeTrue();
            await dbContext.Database.EnsureDeletedAsync();
        }
        
        [Fact]
        public Task ExceptionIsThrownWhenInvalidTypeIsGiven()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new []
                {
                    new KeyValuePair<string, string>("Database:Type", "json"),
                })
                .Build();
            Action serviceProviderCreation = () => new ServiceCollection()
                .AddScoped<IConfiguration>(_ => configuration)
                .AddInfrastructure(configuration)
                .BuildServiceProvider();

            serviceProviderCreation.Should().Throw<DatabaseSettingsException>();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task IsDatabaseAccessibleFromInterfaceAndType()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var serviceProvider = new ServiceCollection()
                .AddScoped<IConfiguration>(_ => configuration)
                .AddInfrastructure(configuration)
                .BuildServiceProvider();

            var dbContext = serviceProvider.GetService<ApplicationDbContext>();
            dbContext.Should().NotBeNull();
            
            await dbContext.Database.EnsureCreatedAsync();
            (await serviceProvider.GetService<ApplicationDbContext>().Database.CanConnectAsync()).Should().BeTrue();
            await dbContext.Database.EnsureDeletedAsync();
            
            var interfacedDbContext = serviceProvider.GetService<IApplicationDbContext>();
            interfacedDbContext.Should().NotBeNull();
        }

        [Fact]
        public Task IsAnyIStopwatchImplementationAvailable()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var serviceProvider = new ServiceCollection()
                .AddScoped<IConfiguration>(_ => configuration)
                .AddInfrastructure(configuration)
                .BuildServiceProvider();

            var stopwatch = serviceProvider.GetService<IStopwatch>();
            stopwatch.Should().NotBeNull();
            return Task.CompletedTask;
        }

        [Fact]
        public Task IsAnyISubstitutionsPlanXmlParserImplementationAvailable()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var serviceProvider = new ServiceCollection()
                .AddScoped<IConfiguration>(_ => configuration)
                .AddInfrastructure(configuration)
                .BuildServiceProvider();

            var xmlParser = serviceProvider.GetService<ISubstitutionsPlanXmlParser>();
            xmlParser.Should().NotBeNull();
            return Task.CompletedTask; 
        }
    }
}