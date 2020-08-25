using System.Threading;
using System.Threading.Tasks;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using FluentAssertions;
using Xunit;

namespace eru.Application.Tests.Subscriptions.Queries
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
                UserId = MockData.ExistingUserId,
                Platform = "DebugMessageService"
            };

            var expected = new Subscriber
            {
                Id = MockData.ExistingUserId, 
                Platform = "DebugMessageService",
                Class = MockData.ExistingClassId,
                PreferredLanguage = "pl"
            };

            var actual = await handler.Handle(request, CancellationToken.None);
            
            actual.Should().BeEquivalentTo(expected);
        }
    }
}