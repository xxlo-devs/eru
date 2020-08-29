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
        public async Task ShouldReturnAllUsersFromGivenPlatformCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new GetIdsOfAllSubscribersInPlatformQueryHandler(context);
            var request = new GetIdsOfAllSubscribersInPlatformQuery("DebugMessageService");

            var expected = new[]
            {
                MockData.ExistingUserId
            };

            var actual = await handler.Handle(request, CancellationToken.None);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}