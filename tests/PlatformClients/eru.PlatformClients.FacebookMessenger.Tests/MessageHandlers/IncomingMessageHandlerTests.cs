using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers
{
    // internal class IncomingMessageHandlerBuilder
    // {
    //     public IncomingMessageHandlerBuilder()
    //     {
    //         SetupServiceProvider();
    //         SetupLogger();
    //         MessageHandler = new IncomingMessageHandler(ServiceProviderMock.Object, LoggerMock.Object);
    //     }
    //
    //     public void VerifyNoOtherCalls()
    //     {
    //         KnownUserMessageHandlerMock.VerifyNoOtherCalls();
    //         RegisteringUserMessageHandlerMock.VerifyNoOtherCalls();
    //         StartRegistrationMessageHandlerMock.VerifyNoOtherCalls();
    //     }
    //     
    //     private void SetupServiceProvider()
    //     {
    //         SetupMediator();
    //         SetupRegistrationDb();
    //         SetupMessageHandling();
    //         
    //         ServiceProviderMock = new Mock<IServiceProvider>();
    //
    //         ServiceProviderMock.Setup(x => x.GetService(typeof(IMediator))).Returns(MediatorMock.Object);
    //         ServiceProviderMock.Setup(x => x.GetService(typeof(IRegistrationDbContext))).Returns(FakeRegistrationDb);
    //         ServiceProviderMock.Setup(x => x.GetService(typeof(KnownUserMessageHandler))).Returns(KnownUserMessageHandlerMock.Object);
    //         ServiceProviderMock.Setup(x => x.GetService(typeof(RegisteringUserMessageHandler))).Returns(RegisteringUserMessageHandlerMock.Object);
    //         ServiceProviderMock.Setup(x => x.GetService(typeof(StartRegistrationMessageHandler))).Returns(StartRegistrationMessageHandlerMock.Object);
    //     }
    //
    //     private void SetupLogger()
    //     {
    //         LoggerMock = new Mock<ILogger<IncomingMessageHandler>>();
    //     }
    //     
    //     private void SetupMediator()
    //     {
    //         MediatorMock = new Mock<IMediator>();
    //         MediatorMock.Setup(x => x.Send(It.IsAny<GetSubscriberQuery>(), It.IsAny<CancellationToken>())).Returns(
    //             (GetSubscriberQuery query, CancellationToken cancellationToken) =>
    //             {
    //                 if (query.Id == "sample-subscriber" && query.Platform == FacebookMessengerPlatformClient.PId)
    //                     return Task.FromResult(new Subscriber
    //                     {
    //                         Id = "sample-subscriber", Platform = FacebookMessengerPlatformClient.PId,
    //                         PreferredLanguage = "en", Class = "sample-class"
    //                     });
    //                 else return Task.FromResult<Subscriber>(null);
    //             });
    //     }
    //
    //     private void SetupRegistrationDb()
    //     {
    //         FakeRegistrationDb = new FakeRegistrationDb();
    //     }
    //
    //     private void SetupMessageHandling()
    //     {
    //         KnownUserMessageHandlerMock = new Mock<KnownUserMessageHandler>();
    //         RegisteringUserMessageHandlerMock = new Mock<RegisteringUserMessageHandler>();
    //         StartRegistrationMessageHandlerMock = new Mock<StartRegistrationMessageHandler>();
    //     }
    //
    //     public IncomingMessageHandler MessageHandler { get; set; }
    //     public Mock<IServiceProvider> ServiceProviderMock { get; set; }
    //     public Mock<ILogger<IncomingMessageHandler>> LoggerMock { get; set; }
    //     
    //     public FakeRegistrationDb FakeRegistrationDb { get; set; }
    //     public Mock<IMediator> MediatorMock { get; set; }
    //     
    //     public Mock<KnownUserMessageHandler> KnownUserMessageHandlerMock { get; set; }
    //     public Mock<RegisteringUserMessageHandler> RegisteringUserMessageHandlerMock { get; set; }
    //     public Mock<StartRegistrationMessageHandler> StartRegistrationMessageHandlerMock { get; set; }
    // }
    public class IncomingMessageHandlerTests
    {
        [Fact]
        public async void ShouldRouteMessageFromKnownUserCorrectly()
        {
            // var builder = new IncomingMessageHandlerBuilder();
            //
            // var message = new Messaging
            // {
            //     Sender = new Sender{ Id = "sample-subscriber" },
            //     Recipient = new Recipient{ Id = "sample-page-id" },
            //     Timestamp = 123456789,
            //     Message = new Message
            //     {
            //         Mid = "sample-message-id",
            //         Text = "sample-message-text"
            //     }
            // };
            //
            // await builder.MessageHandler.Handle(message);
            //
            // builder.KnownUserMessageHandlerMock.Verify(x => x.Handle(message), Times.Once);
            // builder.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldRouteMessageFromRegisteringUserCorrectly()
        {
            
        }

        [Fact]
        public async void ShouldRouteMessageFromUnknownUserCorrectly()
        {
            
        }
    }
}