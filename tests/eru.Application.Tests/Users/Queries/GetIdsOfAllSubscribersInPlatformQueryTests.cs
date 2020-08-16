using System.Threading;
using eru.Application.Users.Queries.GetIdsOfAllSubscribersInPlatform;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using System.Collections.Generic;

namespace eru.Application.Tests.Users.Queries
{
    public class GetIdsOfAllSubscribersInPlatformQueryTests
    {
        [Fact]
        public async Task ShouldReturnAllUsersFromGivenPlatformCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new GetIdsOfAllSubscribersInPlatformQueryHandler(context);
            var request = new GetIdsOfAllSubscribersInPlatformQuery
            {
                Platform = "DebugMessageService"
            };

            var expected = new[]
            {
                "sample-user",
                "sample-user-2",
                "sample-user-3"
            };

            var actual = await handler.Handle(request, CancellationToken.None);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}