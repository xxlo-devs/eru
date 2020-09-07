/*using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.Selector;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.RegisteringUserTests
{
    public class GatherLanguageHandlerTests
    {
        [Fact]
        public async void ShoudGatherLanguageCorrectly()
        {
            var logger = new Mock<ILogger<GatherLanguageMessageHandler>>();
            var context = new FakeRegistrationDb();
            var apiClient = new Mock<ISendApiClient>();
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[] {new KeyValuePair<string, string>("CultureSettings:DefaultCulture", "en"),}).Build();
            var selector = new Mock<ISelector>();
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>(); 
            translator.Setup(x => x.TranslateString("unsupported-command", "en")).Returns(Task.FromResult("This is not a supported command. If you want to delete this bot, just click Cancel. If you want to continue, follow the given instructions."));
            translator.Setup(x => x.TranslateString("greeting", "en")).Returns(Task.FromResult("Hello! Eru is a substitution information system that enables you to get personalized notifications about all substitutions directly from school. If you want to try it, choose your language by clicking on a correct flag below. If you don't want to use this bot, just click Cancel at any time."));
            translator.Setup(x => x.TranslateString("year-selection", "en")).Returns(Task.FromResult("Great! Now you need to select your year, in the same manner."));
            
            var handler = new GatherLanguageMessageHandler(context, apiClient.Object, selector.Object, config, translator.Object, logger.Object);
            await handler.Handle("sample-registering-user", new Payload(PayloadType.Lang, "en"));
        
            context.IncompleteUsers.Should().ContainSingle(x => x.Id == "sample-registering-user" && x.PreferredLanguage == "en" && x.Stage == Stage.GatheredLanguage);
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
            selector.Verify(x => x.GetYearSelector(0, "en"), Times.Once);

        }
    }
}*/