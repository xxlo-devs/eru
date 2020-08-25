using System.Threading;
using System.Threading.Tasks;
using eru.Application.Subscriptions.Queries.GetIdsOfSubscribersInClass;
using FluentAssertions;
using Xunit;

namespace eru.Application.Tests.Subscriptions.Queries
{
    public class GetIdsOfSubscribersInClassQueryTests
    {
        [Fact]
        public async Task ShouldReturnAllUsersFromGivenClassCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new GetIdsOfSubscribersInClassQueryHandler(context);
            var request = new GetIdsOfSubscribersInClassQuery
            {
                Platform = "DebugMessageService",
                Class = MockData.ExistingClassId
            };

            var expected = new[]
            {
                MockData.ExistingUserId
            };

            var actual = await handler.Handle(request, CancellationToken.None);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}