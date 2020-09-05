using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;
using FluentAssertions;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.Selector
{
    public class GetClassSelectorTests
    {
        [Fact]
        public async void ShouldGetClassSelectorCorrectly()
        {
            var builder = new SelectorBuilder();
            builder.InjectClasses();
            
            var selector = new Infrastructure.PlatformClients.FacebookMessenger.Selector.Selector(builder.TranslatorMock.Object, builder.FakeConfigurationBuilder.Build(), builder.MediatorMock.Object);

            var expected = new[]
            {
                new QuickReply("1a1", new Payload(PayloadType.Class, "sample-class-1").ToJson()),
                new QuickReply("1a2", new Payload(PayloadType.Class, "sample-class-7").ToJson()),
                new QuickReply("1b1", new Payload(PayloadType.Class, "sample-class-2").ToJson()),
                new QuickReply("1b2", new Payload(PayloadType.Class, "sample-class-8").ToJson()),
                new QuickReply("1c1", new Payload(PayloadType.Class, "sample-class-3").ToJson()),
                new QuickReply("1c2", new Payload(PayloadType.Class, "sample-class-9").ToJson()),
                new QuickReply("1d1", new Payload(PayloadType.Class, "sample-class-4").ToJson()),
                new QuickReply("1d2", new Payload(PayloadType.Class, "sample-class-10").ToJson()),
                new QuickReply("1e1", new Payload(PayloadType.Class, "sample-class-5").ToJson()),
                new QuickReply("1e2", new Payload(PayloadType.Class, "sample-class-11").ToJson()),
                new QuickReply("->", new Payload(PayloadType.Class, 1).ToJson()),
                new QuickReply("Cancel", new Payload(PayloadType.Cancel).ToJson()),
            };

            var actual = await selector.GetClassSelector(0, 1, "en");

            expected.Should().BeEquivalentTo(actual);
        }
    }
}