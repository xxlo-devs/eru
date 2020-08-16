using System.Threading;
using System.Threading.Tasks;
using eru.Application.Users.Queries.GetUser;
using eru.Domain.Entity;
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
                UserId = "sample-user",
                Platform = "DebugMessageService"
            };

            var expected = new User
            {
                Id = "sample-user", 
                Platform = "DebugMessageService",
                Class = "II b",
                PreferredLanguage = "pl"
            };

            var actual = await handler.Handle(request, CancellationToken.None);
            
            actual.Should().BeEquivalentTo(expected);
        }
    }
}