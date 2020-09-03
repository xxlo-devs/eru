﻿using System.Collections.Generic;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.UnsupportedCommand;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserTests
{
    public class UnsupportedCommandHandlerTests
    {
        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = new Mock<IMediator>();
            var apiClient = new Mock<ISendApiClient>();
            var selector = new Infrastructure.PlatformClients.FacebookMessenger.Selector.Selector();
            
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:0", "en"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:1", "pl"),
                    new KeyValuePair<string, string>("CultureSettings:DefaultCulture", "pl"), 
                }
            ).Build();
            
            var handler = new UnsupportedCommandMessageHandler(context, apiClient.Object, mediator.Object, config, selector);
            await handler.Handle("sample-registering-user");
            
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
    }
}