using System;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.CancelRegistration;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherClass;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherLanguage;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherYear;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using Moq;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.RegisteringUser
{
    internal class RegisteringUserMessageHandlerBuilder
    {
        public RegisteringUserMessageHandlerBuilder()
        {
            RegisteringUserMessageHandler = new RegisteringUserMessageHandler(BuildServiceProvider(),
                MockBuilder.BuildFakeLogger<RegisteringUserMessageHandler>());
        }

        public void VerifyNoOtherCalls()
        {
            CancelRegistrationMessageHandlerMock.VerifyNoOtherCalls();
            ConfirmSubscriptionMessageHandlerMock.VerifyNoOtherCalls();
            GatherLanguageMessageHandlerMock.VerifyNoOtherCalls();
            GatherYearMessageHandlerMock.VerifyNoOtherCalls();
            GatherClassMessageHandler.VerifyNoOtherCalls();
        }
        
        private IServiceProvider BuildServiceProvider()
        {
            FakeRegistrationDb = new FakeRegistrationDb();
            SetupMessageHandling();
            
            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IRegistrationDbContext)))
                .Returns(FakeRegistrationDb);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(ICancelRegistrationMessageHandler)))
                .Returns(CancelRegistrationMessageHandlerMock.Object);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IConfirmSubscriptionMessageHandler)))
                .Returns(ConfirmSubscriptionMessageHandlerMock.Object);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IGatherLanguageMessageHandler)))
                .Returns(GatherLanguageMessageHandlerMock.Object);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IGatherYearMessageHandler)))
                .Returns(GatherYearMessageHandlerMock.Object);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IGatherClassMessageHandler)))
                .Returns(GatherClassMessageHandler.Object);

            return serviceProviderMock.Object;
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
        
        public IRegistrationDbContext FakeRegistrationDb { get; set; }
        
        public Mock<ICancelRegistrationMessageHandler> CancelRegistrationMessageHandlerMock { get; set; } 
        public Mock<IConfirmSubscriptionMessageHandler> ConfirmSubscriptionMessageHandlerMock { get; set; }
        public Mock<IGatherLanguageMessageHandler> GatherLanguageMessageHandlerMock { get; set; }
        public Mock<IGatherYearMessageHandler> GatherYearMessageHandlerMock { get; set; }
        public Mock<IGatherClassMessageHandler> GatherClassMessageHandler { get; set; }

    }
}