using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Commands.CreateSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.Selector;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.RegisteringUserTests
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
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            var logger = new Mock<ILogger>();
            translator.Setup(x => x.TranslateString("congratulations", "en")).Returns(Task.FromResult("Congratulations! You've successfully subscribed to eru Messenger notifications :)"));
            translator.Setup(x => x.TranslateString("unsupported-command", "en")).Returns(Task.FromResult("This is not a supported command. If you want to delete this bot, just click Cancel. If you want to continue, follow the given instructions."));
            
            var selector = new Mock<ISelector>();
            
            var handler = new ConfirmSubscriptionMessageHandler(mediator.Object, context, apiClient.Object, translator.Object, selector.Object, logger.Object);
            await handler.Handle("sample-registering-user-with-class", new Payload(PayloadType.Subscribe));
        
            context.IncompleteUsers.Should().NotContain(x => x.Id == "sample-registering-user-with-class");
            mediator.Verify(x => x.Send(It.IsAny<CreateSubscriptionCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
            selector.Verify(x => x.GetCancelSelector("en"), Times.Once);
        }
    }
}