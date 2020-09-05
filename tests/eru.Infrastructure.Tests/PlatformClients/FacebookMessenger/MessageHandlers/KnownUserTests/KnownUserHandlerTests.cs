using eru.Infrastructure.PlatformClients.FacebookMessenger;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages.Properties;
using Moq;
using Xunit;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.KnownUserTests
{
    public class KnownUserHandlerTests
    {
        // [Fact]
        // public async void ShouldRouteCancelSubscriptionRequestToCancelSubscriptionHandler()
        // {
        //     var cancelSubscriptionHandler = new Mock<ICancelSubscriptionMessageHandler>();
        //     var unsupportedCommandHandler = new Mock<IUnsupportedCommandMessageHandler>();
        //     
        //     var handler = new KnownUserMessageHandler(cancelSubscriptionHandler.Object, unsupportedCommandHandler.Object);
        //     
        //     await handler.Handle("sample-subscriber-id", new Message
        //     {
        //         Mid = "sample-message-id",
        //         Text = "sample-message-text",
        //         QuickReply = new QuickReply{ Payload = ReplyPayloads.CancelPayload }
        //     });
        //     
        //     cancelSubscriptionHandler.Verify(x => x.Handle("sample-subscriber-id"), Times.Once);
        //     unsupportedCommandHandler.Verify(x => x.Handle("sample-subscriber-id"), Times.Never);
        // }
        //
        // [Fact]
        // public async void ShouldRouteRequestWithUnsupportedCommandToUnsupportedCommandHandler()
        // {
        //     var cancelSubscriptionHandler = new Mock<ICancelSubscriptionMessageHandler>();
        //     var unsupportedCommandHandler = new Mock<IUnsupportedCommandMessageHandler>();
        //     
        //     var handler = new KnownUserMessageHandler(cancelSubscriptionHandler.Object, unsupportedCommandHandler.Object);
        //     
        //     await handler.Handle("sample-subscriber-id", new Message
        //     {
        //         Mid = "sample-message-id",
        //         Text = "sample-message-text"
        //     });
        //     
        //     cancelSubscriptionHandler.Verify(x => x.Handle("sample-subscriber-id"), Times.Never);
        //     unsupportedCommandHandler.Verify(x => x.Handle("sample-subscriber-id"), Times.Once);
        // }
    }
}