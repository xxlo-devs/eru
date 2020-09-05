using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherYear;
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
    public class GatherYearHandlerTests
    {
        [Fact]
        public async void ShouldGatherYearCorrectly()
        {
            var context = new FakeRegistrationDb();
            var apiClient = new Mock<ISendApiClient>();
            var selector = new Mock<ISelector>();
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>(); 
            translator.Setup(x => x.TranslateString("unsupported-command", "en")).Returns(Task.FromResult("This is not a supported command. If you want to delete this bot, just click Cancel. If you want to continue, follow the given instructions."));
            translator.Setup(x => x.TranslateString("class-selection", "en")).Returns(Task.FromResult("The last info you need to supply is your class."));
            translator.Setup(x => x.TranslateString("year-selection", "en")).Returns(Task.FromResult("Great! Now you need to select your year, in the same manner."));
            
            var handler = new GatherYearMessageHandler(context, apiClient.Object, selector.Object, translator.Object);
            await handler.Handle("sample-registering-user-with-lang", new Payload(PayloadType.Year, "1"));
        
            context.IncompleteUsers.Should().ContainSingle(x => x.Id == "sample-registering-user-with-lang" && x.Year == 1 && x.PreferredLanguage == "en" && x.Stage == Stage.GatheredYear);
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
            selector.Verify(x => x.GetClassSelector(0, 1, "en"), Times.Once);

        }
    }
}