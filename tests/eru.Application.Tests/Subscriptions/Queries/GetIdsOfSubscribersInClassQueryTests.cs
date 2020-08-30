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
        public async Task ShouldReturnAllSubscribersFromGivenClassCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new GetIdsOfSubscribersInClassQueryHandler(context);
            var request = new GetIdsOfSubscribersInClassQuery(MockData.ExistingClassId,"DebugMessageService");

            var expected = new[]
            {
                MockData.ExistingSubscriberId
            };

            var actual = await handler.Handle(request, CancellationToken.None);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}