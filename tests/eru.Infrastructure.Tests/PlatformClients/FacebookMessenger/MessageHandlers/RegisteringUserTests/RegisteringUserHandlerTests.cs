using System.Globalization;
using System.Threading;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Subscriptions.Commands.CreateSubscription;
using eru.Infrastructure.PlatformClients.FacebookMessenger;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.CancelRegistration;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.ConfirmSubscription;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherClass;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherYear;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;
using Message = eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages.Message;
using QuickReply = eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages.Properties.QuickReply;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserTests
{
    internal class RegisteringUserMessageHandlerBuilder
    {
        public RegisteringUserMessageHandlerBuilder()
        {
            FakeDbContext = new FakeRegistrationDb();
            CancelHandlerMock = new Mock<ICancelRegistrationMessageHandler>();
            ConfirmHandlerMock = new Mock<IConfirmSubscriptionMessageHandler>();
            ClassHandlerMock = new Mock<IGatherClassMessageHandler>();
            LangHandlerMock = new Mock<IGatherLanguageMessageHandler>();
            YearHandlerMock = new Mock<IGatherYearMessageHandler>();
            
            Handler = new RegisteringUserMessageHandler(FakeDbContext, CancelHandlerMock.Object, ConfirmHandlerMock.Object, ClassHandlerMock.Object, LangHandlerMock.Object, YearHandlerMock.Object);
        }
        
        public IRegisteringUserMessageHandler Handler { get; set; }
        public IRegistrationDbContext FakeDbContext { get; set; }
        public Mock<ICancelRegistrationMessageHandler> CancelHandlerMock { get; set; }
        public Mock<IConfirmSubscriptionMessageHandler> ConfirmHandlerMock { get; set; }
        public Mock<IGatherClassMessageHandler> ClassHandlerMock { get; set; }
        public Mock<IGatherLanguageMessageHandler> LangHandlerMock { get; set; }
        public Mock<IGatherYearMessageHandler> YearHandlerMock { get; set; }

        public void VerifyNoOtherCalls()
        {
            CancelHandlerMock.VerifyNoOtherCalls();
            ConfirmHandlerMock.VerifyNoOtherCalls();
            ClassHandlerMock.VerifyNoOtherCalls();
            YearHandlerMock.VerifyNoOtherCalls();
            LangHandlerMock.VerifyNoOtherCalls();
        }
    }
    public class RegisteringUserHandlerTests
    {
        [Fact]
        public async void ShouldRouteCancelRegistrationRequestToAppropriateHandler()
        {
            var builder = new RegisteringUserMessageHandlerBuilder();
        
            await builder.Handler.Handle("sample-registering-user-with-class", new Message
            {
                Mid = "sample-message-id",
                Text = "Cancel",
                QuickReply = new QuickReply {Payload = new Payload(PayloadType.Cancel).ToJson()}
            });
        
            builder.CancelHandlerMock.Verify(x => x.Handle("sample-registering-user-with-class"), Times.Once);
            builder.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async void ShouldRouteGatherLanguageRequestToAppropriateHandler()
        {
            var builder = new RegisteringUserMessageHandlerBuilder();
            
            await builder.Handler.Handle("sample-registering-user", new Message
            {
                Mid = "sample-message-id",
                Text = "English",
                QuickReply = new QuickReply {Payload = new Payload(PayloadType.Lang, "en").ToJson()}
            });
            
            builder.LangHandlerMock.Verify(x => x.Handle("sample-registering-user", It.IsAny<Payload>()), Times.Once);
            builder.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async void ShouldRouteGatherYearRequestToAppropriateHandler()
        {
            var builder = new RegisteringUserMessageHandlerBuilder();
            
            await builder.Handler.Handle("sample-registering-user-with-lang", new Message
            {
                Mid = "sample-message-id",
                Text = "1st Grade",
                QuickReply = new QuickReply {Payload = new Payload(PayloadType.Year, "1").ToJson()}
            });
            
            
            builder.YearHandlerMock.Verify(x => x.Handle("sample-registering-user-with-lang", It.IsAny<Payload>()), Times.Once);
            builder.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async void ShouldRouteGatherClassRequestToAppropriateHandler()
        {
            var builder = new RegisteringUserMessageHandlerBuilder();
            
            await builder.Handler.Handle("sample-registering-user-with-year", new Message
            {
                Mid = "sample-message-id",
                Text = "Ia",
                QuickReply = new QuickReply {Payload = new Payload(PayloadType.Class, "sample-class").ToJson()}
            });
            
            builder.ClassHandlerMock.Verify(x => x.Handle("sample-registering-user-with-year", It.IsAny<Payload>()));
            builder.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async void ShouldRouteConfirmRegistrationRequestToAppropriateHandler()
        {
            var builder = new RegisteringUserMessageHandlerBuilder();
            
            await builder.Handler.Handle("sample-registering-user-with-class", new Message
            {
                Mid = "sample-message-id",
                Text = "Subscribe",
                QuickReply = new QuickReply {Payload = new Payload(PayloadType.Subscribe).ToJson()}
            });
        
            builder.ConfirmHandlerMock.Verify(x => x.Handle("sample-registering-user-with-class", It.IsAny<Payload>()), Times.Once);
            builder.VerifyNoOtherCalls();
        }
    }
}