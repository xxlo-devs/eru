using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Users.Commands.CancelSubscription;
using eru.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace eru.Application.Tests.Users.Commands
{
    public class CancelSubscriptionCommandTest
    {
        [Fact]
        public async Task ShouldCancelSubscriptionCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new CancelSubscriptionCommandHandler(context);
            var request = new CancelSubscriptionCommand
            {
                UserId = "380AE765-803D-4174-A370-1038B7D53CD6",
                Platform = Platform.DebugMessageService
            };

            await handler.Handle(request, CancellationToken.None);

            context.Users.Should().Contain(x =>
                x.Id == "380AE765-803D-4174-A370-1038B7D53CD6" & x.Platform == Platform.DebugMessageService &
                x.Year == Year.NotSupplied & x.Class == String.Empty & x.Stage == Stage.Cancelled);
        }

        [Fact]
        public async Task DoesValidatorAllowCorrectCancelSubscriptionCommand()
        {

        }

        [Fact]
        public async Task DoesValidatorPreventFromUnsubscribingNonExistingUser()
        {

        }
    }
}