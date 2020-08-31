using System.Threading;
using eru.Application.Subscriptions.Commands.CreateSubscription;
using eru.Infrastructure.PlatformClients.FacebookMessenger;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;
using Message = eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages.Message;
using QuickReply = eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages.Properties.QuickReply;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserTests
{
    public class RegisteringUserHandlerTests
    {
        [Fact]
        public async void ShouldRouteUnsupportedCommandRequestToAppropriateHandler()
        {
            var context = new FakeRegistrationDb();
            var apiClient = new Mock<ISendApiClient>();
            
            var handler = new RegisteringUserMessageHandler();
            
            await handler.Handle("sample-registering-user", new Message
            {
                Mid = "sample-message-id",
                Text = "unsupported command! :)"
            });
            
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
        
        [Fact]
        public async void ShouldRouteCancelRegistrationRequestToAppropriateHandler()
        {
            var context = new FakeRegistrationDb();
            var apiClient = new Mock<ISendApiClient>();
            
            var handler = new RegisteringUserMessageHandler();
            
            await handler.Handle("sample-registering-user-with-class", new Message
            {
                Mid = "sample-message-id",
                Text = "Cancel",
                QuickReply = new QuickReply {Payload = ReplyPayloads.CancelPayload}
            });

            context.IncompleteUsers.Should().NotContain(x => x.Id == "sample-registering-user-with-class");
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
        
        [Fact]
        public async void ShouldRouteGatherLanguageRequestToAppropriateHandler()
        {
            var context = new FakeRegistrationDb();
            var apiClient = new Mock<ISendApiClient>();
            
            var handler = new RegisteringUserMessageHandler();
            
            await handler.Handle("sample-registering-user", new Message
            {
                Mid = "sample-message-id",
                Text = "English",
                QuickReply = new QuickReply {Payload = ReplyPayloads.English}
            });

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user" && x.Platform == "FacebookMessenger" && x.PreferredLanguage == "en" && x.Stage == Stage.GatheredLanguage);
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
        
        [Fact]
        public async void ShouldRouteGatherYearRequestToAppropriateHandler()
        {
            var context = new FakeRegistrationDb();
            var apiClient = new Mock<ISendApiClient>();
            
            var handler = new RegisteringUserMessageHandler();
            
            await handler.Handle("sample-registering-user-with-lang", new Message
            {
                Mid = "sample-message-id",
                Text = "1st Grade",
                QuickReply = new QuickReply {Payload = "year:1"}
            });
            
            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-lang" && x.Platform == "FacebookMessenger" && x.PreferredLanguage == "en-us" && x.Year == 1 && x.Stage == Stage.GatheredYear);
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
        
        [Fact]
        public async void ShouldRouteGatherClassRequestToAppropriateHandler()
        {
            var context = new FakeRegistrationDb();
            var apiClient = new Mock<ISendApiClient>();
            
            var handler = new RegisteringUserMessageHandler();
            
            await handler.Handle("sample-registering-user-with-year", new Message
            {
                Mid = "sample-message-id",
                Text = "Ia",
                QuickReply = new QuickReply {Payload = "class:sample-class"}
            });
            
            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-lang" && x.Platform == "FacebookMessenger" && x.PreferredLanguage == "en-us" && x.Year == 1 && x.ClassId == "class:sample-class" && x.Stage == Stage.GatheredYear);
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }

        [Fact]
        public async void ShouldRouteConfirmRegistrationRequestToAppropriateHandler()
        {
            var mediator = new Mock<IMediator>();
            var context = new FakeRegistrationDb();
            var apiClient = new Mock<ISendApiClient>();
            
            var handler = new RegisteringUserMessageHandler();
            
            await handler.Handle("sample-registering-user-with-class", new Message
            {
                Mid = "sample-message-id",
                Text = "Subscribe",
                QuickReply = new QuickReply {Payload = ReplyPayloads.SubscribePayload}
            });

            context.IncompleteUsers.Should().NotContain(x => x.Id == "sample-registering-user-with-class");
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
            mediator.Verify(x => x.Send(It.IsAny<CreateSubscriptionCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}