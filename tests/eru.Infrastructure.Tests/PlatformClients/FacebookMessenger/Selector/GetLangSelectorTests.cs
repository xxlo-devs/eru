using System.Collections.Generic;
using System.Globalization;
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
                new QuickReply(new CultureInfo("en").DisplayName, new Payload(PayloadType.Lang, "en").ToJson()),
                new QuickReply(new CultureInfo("pl").DisplayName, new Payload(PayloadType.Lang, "pl").ToJson()),
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
                new QuickReply(new CultureInfo("sv").DisplayName, new Payload(PayloadType.Lang, "sv").ToJson()),
                new QuickReply(new CultureInfo("sk").DisplayName, new Payload(PayloadType.Lang, "sk").ToJson()),
                new QuickReply(new CultureInfo("ro").DisplayName, new Payload(PayloadType.Lang, "ro").ToJson()),
                new QuickReply(new CultureInfo("pt").DisplayName, new Payload(PayloadType.Lang, "pt").ToJson()),
                new QuickReply(new CultureInfo("de").DisplayName, new Payload(PayloadType.Lang, "de").ToJson()),
                new QuickReply(new CultureInfo("nb").DisplayName, new Payload(PayloadType.Lang, "nb").ToJson()),
                new QuickReply(new CultureInfo("nl").DisplayName, new Payload(PayloadType.Lang, "nl").ToJson()),
                new QuickReply(new CultureInfo("lv").DisplayName, new Payload(PayloadType.Lang, "lv").ToJson()),
                new QuickReply(new CultureInfo("lt").DisplayName, new Payload(PayloadType.Lang, "lt").ToJson()),
                new QuickReply(new CultureInfo("ga").DisplayName, new Payload(PayloadType.Lang, "ga").ToJson()),
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
                new QuickReply(new CultureInfo("en").DisplayName, new Payload(PayloadType.Lang, "en").ToJson()),
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
                new QuickReply(new CultureInfo("es").DisplayName, new Payload(PayloadType.Lang, "es").ToJson()),
                new QuickReply(new CultureInfo("el").DisplayName, new Payload(PayloadType.Lang, "el").ToJson()),
                new QuickReply(new CultureInfo("fr").DisplayName, new Payload(PayloadType.Lang, "fr").ToJson()),
                new QuickReply(new CultureInfo("fi").DisplayName, new Payload(PayloadType.Lang, "fi").ToJson()),
                new QuickReply(new CultureInfo("et").DisplayName, new Payload(PayloadType.Lang, "et").ToJson()),
                new QuickReply(new CultureInfo("da").DisplayName, new Payload(PayloadType.Lang, "da").ToJson()),
                new QuickReply(new CultureInfo("cs").DisplayName, new Payload(PayloadType.Lang, "cs").ToJson()),
                new QuickReply(new CultureInfo("hr").DisplayName, new Payload(PayloadType.Lang, "hr").ToJson()),
                new QuickReply(new CultureInfo("bg").DisplayName, new Payload(PayloadType.Lang, "bg").ToJson()),
                new QuickReply(new CultureInfo("pl").DisplayName, new Payload(PayloadType.Lang, "pl").ToJson()),
                new QuickReply("<-", new Payload(PayloadType.Lang, 0).ToJson()), 
                new QuickReply("->", new Payload(PayloadType.Lang, 2).ToJson()),
                new QuickReply("Cancel", new Payload(PayloadType.Cancel).ToJson()),
            };
            
            var actual = await selector.GetLangSelector(1);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}