using System.Collections.Generic;
using eru.Infrastructure.PlatformClients.FacebookMessenger;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using FluentAssertions;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.Selector
{
    public class SelectorTests
    {
        [Fact]
        public async void CanGenerateSelector()
        {
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < 10; i++)
            {
                dict.Add($"item {i}", $"value {i}");
            }
            var selector = new Infrastructure.PlatformClients.FacebookMessenger.Selector.Selector();
            
            var actual = selector.GetSelector(dict, 0);

            var expected = new[]
            {
                new QuickReply("item 0", "value 0"),
                new QuickReply("item 1", "value 1"),
                new QuickReply("item 2", "value 2"),
                new QuickReply("item 3", "value 3"),
                new QuickReply("item 4", "value 4"),
                new QuickReply("item 5", "value 5"),
                new QuickReply("item 6", "value 6"),
                new QuickReply("item 7", "value 7"),
                new QuickReply("item 8", "value 8"),
                new QuickReply("item 9", "value 9"),
                new QuickReply("Cancel", ReplyPayloads.CancelPayload)
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void CanGenerateSelectorWithNextButton()
        {
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < 15; i++)
            {
                dict.Add($"item {i}", $"value {i}");
            }
            var selector = new Infrastructure.PlatformClients.FacebookMessenger.Selector.Selector();
            
            var actual = selector.GetSelector(dict, 0);

            var expected = new[]
            {
                new QuickReply("item 0", "value 0"),
                new QuickReply("item 1", "value 1"),
                new QuickReply("item 2", "value 2"),
                new QuickReply("item 3", "value 3"),
                new QuickReply("item 4", "value 4"),
                new QuickReply("item 5", "value 5"),
                new QuickReply("item 6", "value 6"),
                new QuickReply("item 7", "value 7"),
                new QuickReply("item 8", "value 8"),
                new QuickReply("item 9", "value 9"),
                new QuickReply("->", ReplyPayloads.NextPage),  
                new QuickReply("Cancel", ReplyPayloads.CancelPayload)
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void CanGenerateSelectorWithPreviousButton()
        {
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < 15; i++)
            {
                dict.Add($"item {i}", $"value {i}");
            }
            var selector = new Infrastructure.PlatformClients.FacebookMessenger.Selector.Selector();
            
            var actual = selector.GetSelector(dict, 10);

            var expected = new[]
            {
                new QuickReply("item 10", "value 10"),
                new QuickReply("item 11", "value 11"),
                new QuickReply("item 12", "value 12"),
                new QuickReply("item 13", "value 13"),
                new QuickReply("item 14", "value 14"),
                new QuickReply("<-", ReplyPayloads.PreviousPage),  
                new QuickReply("Cancel", ReplyPayloads.CancelPayload)
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void CanGenerateBothPageingButtons()
        {
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < 25; i++)
            {
                dict.Add($"item {i}", $"value {i}");
            }
            var selector = new Infrastructure.PlatformClients.FacebookMessenger.Selector.Selector();
            
            var actual = selector.GetSelector(dict, 10);

            var expected = new[]
            {
                new QuickReply("item 10", "value 10"),
                new QuickReply("item 11", "value 11"),
                new QuickReply("item 12", "value 12"),
                new QuickReply("item 13", "value 13"),
                new QuickReply("item 14", "value 14"),
                new QuickReply("item 15", "value 15"),
                new QuickReply("item 16", "value 16"),
                new QuickReply("item 17", "value 17"),
                new QuickReply("item 18", "value 18"),
                new QuickReply("item 19", "value 19"),
                new QuickReply("<-", ReplyPayloads.PreviousPage),
                new QuickReply("->", ReplyPayloads.NextPage), 
                new QuickReply("Cancel", ReplyPayloads.CancelPayload)
            };

            actual.Should().BeEquivalentTo(expected);
        }
    }
}