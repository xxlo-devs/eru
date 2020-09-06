using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.Selector;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.KnownUserTests
{
    public class UnsupportedCommandHandlerTests
    {
        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectlyOnCreatedStage()
        {
            var apiClient = new Mock<ISendApiClient>();
            var selector = new Mock<ISelector>();
            var mediator = new Mock<IMediator>();
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            var logger = new Mock<ILogger<UnsupportedCommandMessageHandler>>();
            
            selector.Setup(x => x.GetCancelSelector("en")).Returns(Task.FromResult(new[] {new QuickReply("Cancel", new Payload(PayloadType.Cancel).ToJson())}.AsEnumerable()));
            
            translator.Setup(x => x.TranslateString("unsupported-command", "en")).Returns(Task.FromResult("This is not a supported command. If you want to delete this bot, just click Cancel. If you want to continue, follow the given instructions."));
            translator.Setup(x => x.TranslateString("cancel-button", "en")).Returns(Task.FromResult("Cancel"));
            
            mediator.Setup(x => x.Send(It.IsAny<GetSubscriberQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Subscriber {Id = "sample-subscriber-id", Platform = FacebookMessengerPlatformClient.PId, Class = "sample-class", PreferredLanguage = "en"}));

            var handler = new UnsupportedCommandMessageHandler(apiClient.Object, translator.Object, mediator.Object, selector.Object, logger.Object);
            
            await handler.Handle("sample-subscriber-id");
            
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
        
    }
}