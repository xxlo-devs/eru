using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Users.Queries.GetIdsOfSubscribersInClass;
using eru.Domain.Enums;
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
                Platform = Platform.DebugMessageService,
                Class = "III c"
            };

            var response = await handler.Handle(request, CancellationToken.None);
            response.Should().HaveCount(1).And.Contain(x => x.Contains("380AE765-803D-4174-A370-1038B7D53CD6"));
        }
    }
}