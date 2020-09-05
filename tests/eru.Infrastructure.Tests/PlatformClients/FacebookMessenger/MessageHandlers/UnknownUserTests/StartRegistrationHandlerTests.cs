using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Infrastructure.PlatformClients.FacebookMessenger;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi.Static;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
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
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:0", "en"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:1", "pl"),
                    new KeyValuePair<string, string>("CultureSettings:DefaultCulture", "pl"), 
                }
            ).Build();
            
            var apiClient = new Mock<ISendApiClient>();
            apiClient.Setup(x => x.Send(It.IsAny<SendRequest>())).Returns((SendRequest x) =>
            {
                if (x.Type != MessagingTypes.Response || x.Tag != null || x.Recipient.Id != "unknown-user") throw new Exception();
                else return Task.CompletedTask;
            });

            var selector = new Mock<ISelector>();
            selector.Setup(x => x.GetLangSelector(0)).Returns(Task.FromResult(new[]
            {
                new QuickReply("English", new Payload(PayloadType.Lang, "en").ToJson()),
                new QuickReply("Polish", new Payload(PayloadType.Lang, "pl").ToJson()),
                new QuickReply("Cancel", new Payload(PayloadType.Cancel).ToJson())
            }.AsEnumerable()));
            
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            translator.Setup(x => x.TranslateString("", "en")).Returns(Task.FromResult(""));
            
            var handler = new StartRegistrationMessageHandler(context, apiClient.Object, config, selector.Object, translator.Object);
            
            await handler.Handle("unknown-user");
        
            context.IncompleteUsers.Should().ContainSingle(x => x.Id == "unknown-user" && x.Stage == Stage.Created);
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Exactly(1));
        }
    }
}