using System;
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
            IncomingMessageHandler = new IncomingMessageHandler(BuildServiceProvider(), MockBuilder.BuildFakeLogger<IncomingMessageHandler>());
        }

        public void VerifyNoOtherCalls()
        {
            KnownUserMessageHandlerMock.VerifyNoOtherCalls();
            RegisteringUserMessageHandlerMock.VerifyNoOtherCalls();
            UnknownUserMessageHandlerMock.VerifyNoOtherCalls();
        }

        private IServiceProvider BuildServiceProvider()
        {
            SetupMessageHandling();
            
            var serviceProviderMock = new Mock<IServiceProvider>();
            
            serviceProviderMock.Setup(x => x.GetService(typeof(IMediator))).Returns(MockBuilder.BuildMediatorMock().Object);
            serviceProviderMock.Setup(x => x.GetService(typeof(IRegistrationDbContext))).Returns(new FakeRegistrationDb());
            serviceProviderMock.Setup(x => x.GetService(typeof(IKnownUserMessageHandler))).Returns(KnownUserMessageHandlerMock.Object);
            serviceProviderMock.Setup(x => x.GetService(typeof(IRegisteringUserMessageHandler))).Returns(RegisteringUserMessageHandlerMock.Object);
            serviceProviderMock.Setup(x => x.GetService(typeof(IUnknownUserMessageHandler))).Returns(UnknownUserMessageHandlerMock.Object);

            return serviceProviderMock.Object;
        }
        
        private void SetupMessageHandling()
        {
            KnownUserMessageHandlerMock = new Mock<IKnownUserMessageHandler>();
            RegisteringUserMessageHandlerMock = new Mock<IRegisteringUserMessageHandler>();
            UnknownUserMessageHandlerMock = new Mock<IUnknownUserMessageHandler>();
        }
        
        public IMessageHandler IncomingMessageHandler { get; set; }
        
        public Mock<IKnownUserMessageHandler> KnownUserMessageHandlerMock { get; set; }
        public Mock<IRegisteringUserMessageHandler> RegisteringUserMessageHandlerMock { get; set; }
        public Mock<IUnknownUserMessageHandler> UnknownUserMessageHandlerMock { get; set; }
    }
    
}