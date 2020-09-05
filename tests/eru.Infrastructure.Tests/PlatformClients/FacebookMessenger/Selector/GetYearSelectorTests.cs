using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;
using FluentAssertions;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.Selector
{
    public class GetYearSelectorTests
    {
        [Fact]
        public async void ShouldGetYearSelectorCorrectly()
        {
            var builder = new SelectorBuilder();
            builder.InjectYears();
            
            var selector = new Infrastructure.PlatformClients.FacebookMessenger.Selector.Selector(builder.TranslatorMock.Object, builder.FakeConfigurationBuilder.Build(), builder.MediatorMock.Object);

            var expected = new[]
            {
                new QuickReply("1", new Payload(PayloadType.Year, "1").ToJson()),
                new QuickReply("2", new Payload(PayloadType.Year, "2").ToJson()),
                new QuickReply("3", new Payload(PayloadType.Year, "3").ToJson()),
                new QuickReply("4", new Payload(PayloadType.Year, "4").ToJson()),
                new QuickReply("5", new Payload(PayloadType.Year, "5").ToJson()),
                new QuickReply("6", new Payload(PayloadType.Year, "6").ToJson()),
                new QuickReply("7", new Payload(PayloadType.Year, "7").ToJson()),
                new QuickReply("8", new Payload(PayloadType.Year, "8").ToJson()),
                new QuickReply("9", new Payload(PayloadType.Year, "9").ToJson()),
                new QuickReply("10", new Payload(PayloadType.Year, "10").ToJson()),
                new QuickReply("->", new Payload(PayloadType.Year, 1).ToJson()),
                new QuickReply("Cancel", new Payload(PayloadType.Cancel).ToJson()),
            };

            var actual = await selector.GetYearSelector(0, "en");

            expected.Should().BeEquivalentTo(actual);
        }
    }
}