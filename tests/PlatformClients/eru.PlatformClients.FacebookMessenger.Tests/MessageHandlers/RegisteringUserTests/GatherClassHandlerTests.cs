using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherClass;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.Selector;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.RegisteringUserTests
{
    public class GatherClassHandlerTests
    {
        [Fact]
        public async void ShouldGatherClassCorrectly()
        {
            var context = new FakeRegistrationDb();
            var apiClient = new Mock<ISendApiClient>();
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            translator.Setup(x => x.TranslateString("confirmation", "en")).Returns(Task.FromResult("Now we have all the required informations to create your subscription. If you want to get a message about all substiututions concerning you as soon as the school publish that information, click the Subscribe button. If you want to delete (or modify) your data, click Cancel."));
            translator.Setup(x => x.TranslateString("unsupported-command", "en")).Returns(Task.FromResult("This is not a supported command. If you want to delete this bot, just click Cancel. If you want to continue, follow the given instructions."));
            translator.Setup(x => x.TranslateString("class-selection", "en")).Returns(Task.FromResult("The last info you need to supply is your class."));
            
            var selector = new Mock<ISelector>();
            
            var handler = new GatherClassMessageHandler(context, apiClient.Object, translator.Object, selector.Object);
            await handler.Handle("sample-registering-user-with-year", new Payload(PayloadType.Class, "sample-class"));
        
            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-year" && x.Year == 1 &&
                x.ClassId == "sample-class" && x.PreferredLanguage == "en" && x.Stage == Stage.GatheredClass);
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
            selector.Verify(x => x.GetConfirmationSelector("en"), Times.Once);
        }
    }
}