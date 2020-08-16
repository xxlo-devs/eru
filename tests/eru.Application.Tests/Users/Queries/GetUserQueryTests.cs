using System.Threading;
using System.Threading.Tasks;
using eru.Application.Users.Queries.GetUser;
using eru.Domain.Entity;
using eru.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace eru.Application.Tests.Users.Queries
{
    public class GetUserQueryTests
    {
        [Fact]
        public async Task ShouldReturnSelectedUserCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new GetUserQueryHandler(context);
            var request = new GetUserQuery
            {
                UserId = "380AE765-803D-4174-A370-1038B7D53CD6",
                Platform = "DebugMessageService"
            };

            var expected = new User
            {
                Id = "380AE765-803D-4174-A370-1038B7D53CD6", 
                Platform = "DebugMessageService",
                Stage = Stage.Subscribed,
                Class = "III c",
                PreferredLanguage = "pl"
            };

            var actual = await handler.Handle(request, CancellationToken.None);
            
            actual.Should().BeEquivalentTo(expected);
        }
    }
}