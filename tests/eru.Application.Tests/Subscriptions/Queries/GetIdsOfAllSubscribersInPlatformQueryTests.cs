using System.Threading;
using System.Threading.Tasks;
using eru.Application.Subscriptions.Queries.GetIdsOfAllSubscribersInPlatform;
using FluentAssertions;
using Xunit;

namespace eru.Application.Tests.Subscriptions.Queries
{
    public class GetIdsOfAllSubscribersInPlatformQueryTests
    {
        [Fact]
        public async Task ShouldReturnAllSubscribersFromGivenPlatformCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new GetIdsOfAllSubscribersInPlatformQueryHandler(context);
            var request = new GetIdsOfAllSubscribersInPlatformQuery("DebugMessageService");

            var expected = new[]
            {
                MockData.ExistingSubscriberId
            };

            var actual = await handler.Handle(request, CancellationToken.None);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}