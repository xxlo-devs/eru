using System.Collections.Generic;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherLanguage;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.UnknownUser
{
    public class StartRegistrationMessageHandlerTests
    {
        [Fact]
        public async void ShouldStartRegistrationCorrectly()
        {
            var context = new FakeRegistrationDb();
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[] {new KeyValuePair<string, string>("CultureSettings:DefaultCulture", "en")}).Build();
            var langHandlerMock = new Mock<IGatherLanguageMessageHandler>();
            var loggerMock = new Mock<ILogger<StartRegistrationMessageHandler>>();

            var message = new Messaging
            {
                Sender = new Sender{Id = "sample-unknown-user"},
                Recipient = new Recipient{Id = "sample-page-id"},
                Timestamp = 123456789,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text"
                }
            };
            
            var handler = new StartRegistrationMessageHandler(context, config, langHandlerMock.Object, loggerMock.Object);
            await handler.Handle(message);

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-unknown-user" && x.PreferredLanguage == "en" && x.Year == 0 && x.ClassId == null &&
                x.Stage == Stage.Created && x.LastPage == 0);
            var contextUser = await context.IncompleteUsers.FindAsync("sample-unknown-user");
            langHandlerMock.Verify(x => x.ShowInstruction(contextUser, 0));
            langHandlerMock.VerifyNoOtherCalls();
        }
    }
}