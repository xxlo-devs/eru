using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.Infrastructure.PlatformClients.FacebookMessenger;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.KnownUserTests
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

            selector.Setup(x => x.GetCancelSelector("en")).Returns(Task.FromResult(new[] {new QuickReply("Cancel", new Payload(PayloadType.Cancel).ToJson())}.AsEnumerable()));
            
            translator.Setup(x => x.TranslateString("unsupported-command", "en")).Returns(Task.FromResult("This is not a supported command. If you want to delete this bot, just click Cancel. If you want to continue, follow the given instructions."));
            translator.Setup(x => x.TranslateString("cancel-button", "en")).Returns(Task.FromResult("Cancel"));
            
            mediator.Setup(x => x.Send(It.IsAny<GetSubscriberQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Subscriber {Id = "sample-subscriber-id", Platform = FacebookMessengerPlatformClient.PId, Class = "sample-class", PreferredLanguage = "en"}));
            
            
            var handler = new UnsupportedCommandMessageHandler(apiClient.Object, translator.Object, mediator.Object, selector.Object);
            
            await handler.Handle("sample-subscriber-id");
            
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
        
    }
}