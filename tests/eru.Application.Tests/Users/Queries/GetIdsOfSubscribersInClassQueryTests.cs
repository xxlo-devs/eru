using System.Threading;
using System.Threading.Tasks;
using eru.Application.Users.Queries.GetIdsOfSubscribersInClass;
using FluentAssertions;
using Xunit;

namespace eru.Application.Tests.Users.Queries
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
                Class = "II b"
            };

            var response = await handler.Handle(request, CancellationToken.None);
            response.Should().HaveCount(1).And.Contain(x => x.Contains("sample-user"));
        }
    }
}