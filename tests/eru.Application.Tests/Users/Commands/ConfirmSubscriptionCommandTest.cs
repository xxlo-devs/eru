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
                Platform = "DebugMessageService"
            };

            await handler.Handle(request, CancellationToken.None);

            context.Users.Should().Contain(x =>
                x.Id == "7124C49B-B04A-468F-A946-40025B19FF91" & x.Platform == "DebugMessageService" & x.Class == "II b" & x.Stage == Stage.Subscribed);

        }

        [Fact]
        public async Task DoesValidatorAllowCorrectConfirmSubscribeCommand()
        {
            var context = new FakeDbContext();
            var validator = new ConfirmSubscriptionCommandValidator(context);
            var request = new ConfirmSubscriptionCommand
            {
                UserId = "7124C49B-B04A-468F-A946-40025B19FF91",
                Platform = "DebugMessageService"
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public async Task DoesValidatorPreventFromConfirmingNonExistingAccount()
        {
            var context = new FakeDbContext();
            var validator = new ConfirmSubscriptionCommandValidator(context);
            var request = new ConfirmSubscriptionCommand
            {
                UserId = "E888C90C-00DA-41CF-837D-8F037DA03F11",
                Platform = "DebugMessageService"
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == "The specified condition was not met for ''.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromConfirmingAccountOnInvalidStage()
        {
            var context = new FakeDbContext();
            var validator = new ConfirmSubscriptionCommandValidator(context);
            var request = new ConfirmSubscriptionCommand
            {
                UserId = "380AE765-803D-4174-A370-1038B7D53CD6",
                Platform = "DebugMessageService"
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == "The specified condition was not met for ''.");
        }
    }
}