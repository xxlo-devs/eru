using System.Threading;
using System.Threading.Tasks;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using FluentAssertions;
using Xunit;

namespace eru.Application.Tests.Subscriptions.Queries
{
    public class GetSubscriberQueryTests
    {
        [Fact]
        public async Task ShouldReturnSelectedSubscriberCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new GetSubscriberQueryHandler(context);
            var request = new GetSubscriberQuery(MockData.ExistingSubscriberId, "DebugMessageService");

            var expected = new Subscriber
            {
                Id = MockData.ExistingSubscriberId, 
                Platform = "DebugMessageService",
                Class = MockData.ExistingClassId,
                PreferredLanguage = "pl"
            };

            var actual = await handler.Handle(request, CancellationToken.None);
            
            actual.Should().BeEquivalentTo(expected);
        }
    }
}