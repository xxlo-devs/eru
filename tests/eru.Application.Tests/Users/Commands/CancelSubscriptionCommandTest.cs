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
                Platform = "DebugMessageService"
            };

            await handler.Handle(request, CancellationToken.None);

            context.Users.Should().Contain(x =>
                x.Id == "380AE765-803D-4174-A370-1038B7D53CD6" & x.Platform == "DebugMessageService" & x.Class == String.Empty & x.Stage == Stage.Cancelled);
        }

        [Fact]
        public async Task DoesValidatorAllowCorrectCancelSubscriptionCommand()
        {
            var context = new FakeDbContext();
            var validator = new CancelSubscriptionCommandValidator(context);
            var request = new CancelSubscriptionCommand
            {
                UserId = "380AE765-803D-4174-A370-1038B7D53CD6",
                Platform = "DebugMessageService"
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public async Task DoesValidatorPreventFromUnsubscribingNonExistingUser()
        {
            var context = new FakeDbContext();
            var validator = new CancelSubscriptionCommandValidator(context);
            var request = new CancelSubscriptionCommand
            {
                UserId = "38C12C16-2A68-4F56-B434-62A382DB4DA0",
                Platform = "DebugMessageService"
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2).And.Contain(x => x.ErrorMessage == "The specified condition was not met for ''.");
        }

        [Fact]
        public async Task DoesValidatorPreventsFromUnsubscribingAlreadyUnsubscribedUser()
        {
            var context = new FakeDbContext();
            var validator = new CancelSubscriptionCommandValidator(context);
            var request = new CancelSubscriptionCommand
            {
                UserId = "FCDEE5DA-F755-45F9-B8BB-D7C7C303F70B",
                Platform = "DebugMessageService"
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.Contain(x => x.ErrorMessage == "The specified condition was not met for ''.");
        }
    }
}