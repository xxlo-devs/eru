using System.Collections.Generic;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.Selector
{
    public class GetLangSelectorTests
    {
        [Fact]
        public async void ShouldGetSelectorCorrectly()
        {
            var builder = new SelectorBuilder();
            builder.FakeConfigurationBuilder.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("CultureSettings:DefaultCulture", "en"),
                new KeyValuePair<string, string>("CultureSettings:AvailableCultures:0", "en"),
                new KeyValuePair<string, string>("CultureSettings:AvailableCultures:1", "pl"),
            });
            
            var selector = new Infrastructure.PlatformClients.FacebookMessenger.Selector.Selector(builder.TranslatorMock.Object, builder.FakeConfigurationBuilder.Build(), builder.MediatorMock.Object);

            var expected = new[]
            {
                new QuickReply("English", new Payload(PayloadType.Lang, "en").ToJson()),
                new QuickReply("Polish", new Payload(PayloadType.Lang, "pl").ToJson()),
                new QuickReply("Cancel", new Payload(PayloadType.Cancel).ToJson())
            };

            var actual = await selector.GetLangSelector(0);

            actual.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public async void ShouldGetLangSelectorWithNextPageButtonCorrectly()
        {
            var builder = new SelectorBuilder();
            builder.InjectCultures();
            
            var selector = new Infrastructure.PlatformClients.FacebookMessenger.Selector.Selector(builder.TranslatorMock.Object, builder.FakeConfigurationBuilder.Build(), builder.MediatorMock.Object);

            var expected = new[]
            {
                new QuickReply("Swedish", new Payload(PayloadType.Lang, "sv").ToJson()),
                new QuickReply("Slovak", new Payload(PayloadType.Lang, "sk").ToJson()),
                new QuickReply("Romanian", new Payload(PayloadType.Lang, "ro").ToJson()),
                new QuickReply("Portuguese", new Payload(PayloadType.Lang, "pt").ToJson()),
                new QuickReply("German", new Payload(PayloadType.Lang, "de").ToJson()),
                new QuickReply("Norwegian Bokmål", new Payload(PayloadType.Lang, "nb").ToJson()),
                new QuickReply("Dutch", new Payload(PayloadType.Lang, "nl").ToJson()),
                new QuickReply("Latvian", new Payload(PayloadType.Lang, "lv").ToJson()),
                new QuickReply("Lithuanian", new Payload(PayloadType.Lang, "lt").ToJson()),
                new QuickReply("Irish", new Payload(PayloadType.Lang, "ga").ToJson()),
                new QuickReply("->", new Payload(PayloadType.Lang, 1).ToJson()),
                new QuickReply("Cancel", new Payload(PayloadType.Cancel).ToJson()),
            };
            
            var actual = await selector.GetLangSelector(0);

            actual.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public async void ShouldGetLangSelectorWithPreviousPageButtonCorrectly()
        {
            var builder = new SelectorBuilder();
            builder.InjectCultures();
            
            var selector = new Infrastructure.PlatformClients.FacebookMessenger.Selector.Selector(builder.TranslatorMock.Object, builder.FakeConfigurationBuilder.Build(), builder.MediatorMock.Object);

            var expected = new[]
            {
                new QuickReply("English", new Payload(PayloadType.Lang, "en").ToJson()),
                new QuickReply("<-", new Payload(PayloadType.Lang, 1).ToJson()),
                new QuickReply("Cancel", new Payload(PayloadType.Cancel).ToJson()) 
            };
            
            var actual = await selector.GetLangSelector(2);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void ShouldGetLangSelectorWithBothButtonsCorrectly()
        {
            var builder = new SelectorBuilder();
            builder.InjectCultures();
            
            var selector = new Infrastructure.PlatformClients.FacebookMessenger.Selector.Selector(builder.TranslatorMock.Object, builder.FakeConfigurationBuilder.Build(), builder.MediatorMock.Object);

            var expected = new[]
            {
                new QuickReply("Spanish", new Payload(PayloadType.Lang, "es").ToJson()),
                new QuickReply("Greek", new Payload(PayloadType.Lang, "el").ToJson()),
                new QuickReply("French", new Payload(PayloadType.Lang, "fr").ToJson()),
                new QuickReply("Finnish", new Payload(PayloadType.Lang, "fi").ToJson()),
                new QuickReply("Estonian", new Payload(PayloadType.Lang, "et").ToJson()),
                new QuickReply("Danish", new Payload(PayloadType.Lang, "da").ToJson()),
                new QuickReply("Czech", new Payload(PayloadType.Lang, "cs").ToJson()),
                new QuickReply("Croatian", new Payload(PayloadType.Lang, "hr").ToJson()),
                new QuickReply("Bulgarian", new Payload(PayloadType.Lang, "bg").ToJson()),
                new QuickReply("Polish", new Payload(PayloadType.Lang, "pl").ToJson()),
                new QuickReply("<-", new Payload(PayloadType.Lang, 0).ToJson()), 
                new QuickReply("->", new Payload(PayloadType.Lang, 2).ToJson()),
                new QuickReply("Cancel", new Payload(PayloadType.Cancel).ToJson()),
            };
            
            var actual = await selector.GetLangSelector(1);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}