using System;
using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi.Static;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
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
            
            var apiClient = new Mock<ISendApiClient>();
            apiClient.Setup(x => x.Send(It.IsAny<SendRequest>())).Returns((SendRequest x) =>
            {
                if (x.Type != MessagingTypes.Response || x.Tag != null || x.Recipient.Id != "unknown-user") throw new Exception();
                else return Task.CompletedTask;
            });
            
            var handler = new StartRegistrationMessageHandler(context, apiClient.Object);
            
            await handler.Handle("unknown-user");

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "unknown-user" && x.Platform == "FacebookMessenger" && x.Stage == Stage.Created);
            
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Exactly(1));
        }
    }
}