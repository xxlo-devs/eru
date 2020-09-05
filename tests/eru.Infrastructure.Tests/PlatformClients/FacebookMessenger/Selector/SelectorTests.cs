using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Infrastructure.PlatformClients.FacebookMessenger;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.Selector
{
    public class SelectorTests
    {
        [Fact]
        public async void ShouldReturnConfirmationSelectorCorrectly()
        {
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            translator.Setup(x => x.TranslateString("cancel-button", "en")).Returns(Task.FromResult("Cancel"));
            translator.Setup(x => x.TranslateString("subscribe-button", "en")).Returns(Task.FromResult("Subscribe"));
            var mediator = new Mock<IMediator>();
            var config = new ConfigurationBuilder().Build();
            var selector = new Infrastructure.PlatformClients.FacebookMessenger.Selector.Selector(translator.Object, config, mediator.Object);

            var expected = new[]
            {
                new QuickReply("Subscribe", new Payload(PayloadType.Subscribe).ToJson()),
                new QuickReply("Cancel", new Payload(PayloadType.Cancel).ToJson())
            };
            
            var actual = await selector.GetConfirmationSelector("en");

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void ShouldReturnCancelSelectorCorrectly()
        {
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            translator.Setup(x => x.TranslateString("cancel-button", "en")).Returns(Task.FromResult("Cancel"));
            var mediator = new Mock<IMediator>();
            var config = new ConfigurationBuilder().Build();
            var selector = new Infrastructure.PlatformClients.FacebookMessenger.Selector.Selector(translator.Object, config, mediator.Object);

            var expected = new[]
            {
                new QuickReply("Cancel", new Payload(PayloadType.Cancel).ToJson())
            };
            
            var actual = await selector.GetCancelSelector("en");

            actual.Should().BeEquivalentTo(expected);
        }
    }
}