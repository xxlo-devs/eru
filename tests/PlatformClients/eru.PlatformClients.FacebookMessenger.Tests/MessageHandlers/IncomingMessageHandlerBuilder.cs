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
            BuildServiceProvider();
            FakeLogger = MockBuilder.BuildFakeLogger<IncomingMessageHandler>();
            
            IncomingMessageHandler = new IncomingMessageHandler(ServiceProviderMock.Object, FakeLogger);
        }

        public void VerifyNoOtherCalls()
        {
            KnownUserMessageHandlerMock.VerifyNoOtherCalls();
            RegisteringUserMessageHandlerMock.VerifyNoOtherCalls();
            UnknownUserMessageHandlerMock.VerifyNoOtherCalls();
        }

        private void BuildServiceProvider()
        {
            MediatorMock = MockBuilder.BuildMediatorMock();
            FakeRegistrationDb = new FakeRegistrationDb();
            
            SetupMessageHandler();
            
            ServiceProviderMock = new Mock<IServiceProvider>();
            
            ServiceProviderMock.Setup(x => x.GetService(typeof(IMediator))).Returns(MediatorMock.Object);
            ServiceProviderMock.Setup(x => x.GetService(typeof(IRegistrationDbContext))).Returns(FakeRegistrationDb);
            ServiceProviderMock.Setup(x => x.GetService(typeof(IKnownUserMessageHandler))).Returns(KnownUserMessageHandlerMock.Object);
            ServiceProviderMock.Setup(x => x.GetService(typeof(IRegisteringUserMessageHandler))).Returns(RegisteringUserMessageHandlerMock.Object);
            ServiceProviderMock.Setup(x => x.GetService(typeof(IUnknownUserMessageHandler))).Returns(UnknownUserMessageHandlerMock.Object);
        }
        
        private void SetupMessageHandler()
        {
            KnownUserMessageHandlerMock = new Mock<IKnownUserMessageHandler>();
            RegisteringUserMessageHandlerMock = new Mock<IRegisteringUserMessageHandler>();
            UnknownUserMessageHandlerMock = new Mock<IUnknownUserMessageHandler>();
        }
        
        public IMessageHandler IncomingMessageHandler { get; set; }
        
        public Mock<IKnownUserMessageHandler> KnownUserMessageHandlerMock { get; set; }
        public Mock<IRegisteringUserMessageHandler> RegisteringUserMessageHandlerMock { get; set; }
        public Mock<IUnknownUserMessageHandler> UnknownUserMessageHandlerMock { get; set; }
        
        private Mock<IServiceProvider> ServiceProviderMock { get; set; }
        private ILogger<IncomingMessageHandler> FakeLogger { get; set; }
        private Mock<IMediator> MediatorMock { get; set; }
        private IRegistrationDbContext FakeRegistrationDb { get; set; }
    }
    
}