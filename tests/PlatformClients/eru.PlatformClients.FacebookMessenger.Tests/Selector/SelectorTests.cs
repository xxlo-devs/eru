using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.Selector
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
            var selector = new FacebookMessenger.Selector.Selector(translator.Object, config, mediator.Object);

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
            var selector = new FacebookMessenger.Selector.Selector(translator.Object, config, mediator.Object);

            var expected = new[]
            {
                new QuickReply("Cancel", new Payload(PayloadType.Cancel).ToJson())
            };
            
            var actual = await selector.GetCancelSelector("en");

            actual.Should().BeEquivalentTo(expected);
        }
    }
}