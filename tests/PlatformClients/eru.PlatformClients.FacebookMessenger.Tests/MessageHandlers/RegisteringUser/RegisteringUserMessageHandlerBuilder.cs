using System;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.CancelRegistration;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherClass;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherLanguage;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherYear;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using Microsoft.Extensions.Logging;
using Moq;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.RegisteringUser
{
    internal class RegisteringUserMessageHandlerBuilder
    {
        public RegisteringUserMessageHandlerBuilder()
        {
            BuildServiceProvider();
            FakeLogger = MockBuilder.BuildFakeLogger<RegisteringUserMessageHandler>();
            
            RegisteringUserMessageHandler = new RegisteringUserMessageHandler(ServiceProviderMock.Object, FakeLogger);
        }

        public void VerifyNoOtherCalls()
        {
            CancelRegistrationMessageHandlerMock.VerifyNoOtherCalls();
            ConfirmSubscriptionMessageHandlerMock.VerifyNoOtherCalls();
            GatherLanguageMessageHandlerMock.VerifyNoOtherCalls();
            GatherYearMessageHandlerMock.VerifyNoOtherCalls();
            GatherClassMessageHandler.VerifyNoOtherCalls();
        }
        
        private void BuildServiceProvider()
        {
            FakeRegistrationDb = new FakeRegistrationDb();
            SetupMessageHandling();
            
            ServiceProviderMock = new Mock<IServiceProvider>();

            ServiceProviderMock.Setup(x => x.GetService(typeof(IRegistrationDbContext))).Returns(FakeRegistrationDb);
            ServiceProviderMock.Setup(x => x.GetService(typeof(ICancelRegistrationMessageHandler))).Returns(CancelRegistrationMessageHandlerMock.Object);
            ServiceProviderMock.Setup(x => x.GetService(typeof(IConfirmSubscriptionMessageHandler))).Returns(ConfirmSubscriptionMessageHandlerMock.Object);
            ServiceProviderMock.Setup(x => x.GetService(typeof(IGatherLanguageMessageHandler))).Returns(GatherLanguageMessageHandlerMock.Object);
            ServiceProviderMock.Setup(x => x.GetService(typeof(IGatherYearMessageHandler))).Returns(GatherYearMessageHandlerMock.Object);
            ServiceProviderMock.Setup(x => x.GetService(typeof(IGatherClassMessageHandler))).Returns(GatherClassMessageHandler.Object);
        }
        
        private void SetupMessageHandling()
        {
            CancelRegistrationMessageHandlerMock = new Mock<ICancelRegistrationMessageHandler>();
            ConfirmSubscriptionMessageHandlerMock = new Mock<IConfirmSubscriptionMessageHandler>();
            GatherLanguageMessageHandlerMock = new Mock<IGatherLanguageMessageHandler>();
            GatherYearMessageHandlerMock = new Mock<IGatherYearMessageHandler>();
            GatherClassMessageHandler = new Mock<IGatherClassMessageHandler>();
        }
        
        public IRegisteringUserMessageHandler RegisteringUserMessageHandler { get; set; }
        
        public Mock<IServiceProvider> ServiceProviderMock { get; set; }
        public ILogger<RegisteringUserMessageHandler> FakeLogger { get; set; } 
        
        public IRegistrationDbContext FakeRegistrationDb { get; set; }
        
        public Mock<ICancelRegistrationMessageHandler> CancelRegistrationMessageHandlerMock { get; set; } 
        public Mock<IConfirmSubscriptionMessageHandler> ConfirmSubscriptionMessageHandlerMock { get; set; }
        public Mock<IGatherLanguageMessageHandler> GatherLanguageMessageHandlerMock { get; set; }
        public Mock<IGatherYearMessageHandler> GatherYearMessageHandlerMock { get; set; }
        public Mock<IGatherClassMessageHandler> GatherClassMessageHandler { get; set; }
    }
}