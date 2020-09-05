﻿using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Infrastructure.PlatformClients.FacebookMessenger;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.CancelRegistration;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserTests
{
    public class CancelRegistrationHandlerTests
    {
        [Fact]
        public async void ShouldCancelRegistrationCorrectly()
        {
            var context = new FakeRegistrationDb();
            var apiClient = new Mock<ISendApiClient>();
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            translator.Setup(x => x.TranslateString("subscription-cancelled", "en")).Returns(Task.FromResult("We are sorry to see you go. Your subscription (and your data) has been deleted. If you will ever want to subscribe again, write anything to start the registration process."));
            
            var handler = new CancelRegistrationMessageHandler(context, apiClient.Object, translator.Object);
            await handler.Handle("sample-registering-user-with-class");
        
            context.IncompleteUsers.Should().NotContain(x => x.Id == "sample-registering-user-with-class");
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
    }
}