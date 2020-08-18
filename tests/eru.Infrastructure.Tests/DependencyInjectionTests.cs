﻿using System;
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
                .AddInfrastructure(configuration)
                .BuildServiceProvider();

            var stopwatch = serviceProvider.GetService<IStopwatch>();
            stopwatch.Should().NotBeNull();
            return Task.CompletedTask;
        }

        [Fact]
        public Task IsHangfireCorrectlyConfigured()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
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