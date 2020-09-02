using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi.Static;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUserTests
{
    public class StartRegistrationHandlerTests
    {
        [Fact]
        public async void ShouldStartRegistration()
        {
            var context = new FakeRegistrationDb();
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:0", "en"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:1", "pl"),
                    new KeyValuePair<string, string>("CultureSettings:DefaultCulture", "pl"), 
                }
            ).Build();
            
            var apiClient = new Mock<ISendApiClient>();
            apiClient.Setup(x => x.Send(It.IsAny<SendRequest>())).Returns((SendRequest x) =>
            {
                if (x.Type != MessagingTypes.Response || x.Tag != null || x.Recipient.Id != "unknown-user") throw new Exception();
                else return Task.CompletedTask;
            });
            
            var selector = new Infrastructure.PlatformClients.FacebookMessenger.Selector.Selector();
            
            var handler = new StartRegistrationMessageHandler(context, apiClient.Object, config, selector);
            
            await handler.Handle("unknown-user");

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "unknown-user" && x.Platform == "FacebookMessenger" && x.Stage == Stage.Created);
            
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Exactly(1));
        }
    }
}