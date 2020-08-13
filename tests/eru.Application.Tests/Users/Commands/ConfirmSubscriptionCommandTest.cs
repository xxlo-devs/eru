using System.Threading;
using System.Threading.Tasks;
using eru.Application.Users.Commands.ConfirmSubscription;
using eru.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace eru.Application.Tests.Users.Commands
{
    public class ConfirmSubscriptionCommandTest
    {
        [Fact]
        public async Task ShouldConfirmSubscriptionCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new ConfirmSubscriptionCommandHandler(context);
            var request = new ConfirmSubscriptionCommand
            {
                UserId = "7124C49B-B04A-468F-A946-40025B19FF91",
                Platform = Platform.DebugMessageService
            };

            await handler.Handle(request, CancellationToken.None);

            context.Users.Should().Contain(x =>
                x.Id == "7124C49B-B04A-468F-A946-40025B19FF91" & x.Platform == Platform.DebugMessageService &
                x.Year == Year.Sophomore & x.Class == "język Polski" & x.Stage == Stage.Subscribed);

        }
    }
}