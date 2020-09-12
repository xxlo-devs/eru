using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers
{
    internal class IncomingMessageHandlerBuilder
    {
        public IncomingMessageHandlerBuilder()
        {
            BuildServiceProvider();
            SetupLogger();
            
            IncomingMessageHandler = new IncomingMessageHandler(ServiceProviderMock.Object, LoggerMock.Object);
        }

        public void VerifyNoOtherCalls()
        {
            KnownUserMessageHandlerMock.VerifyNoOtherCalls();
            RegisteringUserMessageHandlerMock.VerifyNoOtherCalls();
            UnknownUserMessageHandlerMock.VerifyNoOtherCalls();
        }

        private void BuildServiceProvider()
        {
            SetupMediatorMock();
            SetupFakeRegistrationDb();
            SetupMessageHandler();
            
            ServiceProviderMock = new Mock<IServiceProvider>();
            
            ServiceProviderMock.Setup(x => x.GetService(typeof(IMediator))).Returns(MediatorMock.Object);
            ServiceProviderMock.Setup(x => x.GetService(typeof(IRegistrationDbContext))).Returns(FakeRegistrationDb);
            ServiceProviderMock.Setup(x => x.GetService(typeof(IKnownUserMessageHandler))).Returns(KnownUserMessageHandlerMock.Object);
            ServiceProviderMock.Setup(x => x.GetService(typeof(IRegisteringUserMessageHandler))).Returns(RegisteringUserMessageHandlerMock.Object);
            ServiceProviderMock.Setup(x => x.GetService(typeof(IUnknownUserMessageHandler))).Returns(UnknownUserMessageHandlerMock.Object);
        }

        private void SetupLogger()
        {
            LoggerMock = new Mock<ILogger<IncomingMessageHandler>>();
        }
        
        private void SetupMediatorMock()
        {
            MediatorMock = new Mock<IMediator>();
            MediatorMock.Setup(x => x.Send(It.IsAny<GetSubscriberQuery>(), It.IsAny<CancellationToken>())).Returns(
                (GetSubscriberQuery query, CancellationToken cancellationToken) =>
                {
                    if (query.Id == "sample-subscriber" && query.Platform == FacebookMessengerPlatformClient.PId)
                        return Task.FromResult(new Subscriber
                        {
                            Id = "sample-subscriber", Platform = FacebookMessengerPlatformClient.PId,
                            PreferredLanguage = "en", Class = "sample-class"
                        });
                    else
                        return Task.FromResult<Subscriber>(null);
                });
        }
        
        private void SetupFakeRegistrationDb()
        {
            FakeRegistrationDb = new FakeRegistrationDb();
        }

        private void SetupMessageHandler()
        {
            KnownUserMessageHandlerMock = new Mock<IKnownUserMessageHandler>();
            RegisteringUserMessageHandlerMock = new Mock<IRegisteringUserMessageHandler>();
            UnknownUserMessageHandlerMock = new Mock<IUnknownUserMessageHandler>();
        }
        
        public IMessageHandler IncomingMessageHandler { get; set; }
        public Mock<IServiceProvider> ServiceProviderMock { get; set; }
        public Mock<ILogger<IncomingMessageHandler>> LoggerMock { get; set; }
        
        public Mock<IMediator> MediatorMock { get; set; }
        public IRegistrationDbContext FakeRegistrationDb { get; set; }
        
        public Mock<IKnownUserMessageHandler> KnownUserMessageHandlerMock { get; set; }
        public Mock<IRegisteringUserMessageHandler> RegisteringUserMessageHandlerMock { get; set; }
        public Mock<IUnknownUserMessageHandler> UnknownUserMessageHandlerMock { get; set; }
    }
    
}