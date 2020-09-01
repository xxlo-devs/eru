using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Subscriptions.Commands.CreateSubscription;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.ConfirmSubscription;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserTests
{
    public class ConfirmSubscriptionHandlerTests
    {
        [Fact]
        public async void ShouldConfirmSubscriptionCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CreateSubscriptionCommand>(), It.IsAny<CancellationToken>())).Returns(
                (CreateSubscriptionCommand command, CancellationToken cancellationToken) =>
                {
                    if(command.Id != "sample-registering-user-with-class" || command.Platform != "FacebookMessenger" || command.Class != "sample-class" || command.PreferredLanguage != "en") throw new Exception();
                    return Task.FromResult<Unit>(Unit.Value);
                });
            var apiClient = new Mock<ISendApiClient>();
            
            var handler = new ConfirmSubscriptionMessageHandler(mediator.Object, context, apiClient.Object);
            await handler.Handle("sample-registering-user-with-class");

            context.IncompleteUsers.Should().NotContain(x => x.Id == "sample-registering-user-with-class");
            mediator.Verify(x => x.Send(It.IsAny<CreateSubscriptionCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
    }
}