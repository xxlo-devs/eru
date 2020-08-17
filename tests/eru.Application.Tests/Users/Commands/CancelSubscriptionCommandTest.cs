using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Users.Commands.CancelSubscription;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace eru.Application.Tests.Users.Commands
{
    public class CancelSubscriptionCommandTest
    {
        public CancelSubscriptionCommandTest()
        {
            ValidatorOptions.Global.LanguageManager.Enabled = false;
        }
        [Fact]
        public async Task ShouldCancelSubscriptionCorrectly()
        {
            var context = new FakeDbContext();

            var handler = new CancelSubscriptionCommandHandler(context);
            var request = new CancelSubscriptionCommand
            {
                UserId = "sample-user",
                Platform = "DebugMessageService"
            };

            await handler.Handle(request, CancellationToken.None);

            context.Users.Should().NotContain(x => x.Id == "sample-user" & x.Platform == "DebugMessageService");
        }

        [Fact]
        public async Task DoesValidatorAllowCorrectCancelSubscriptionCommand()
        {
            var context = new FakeDbContext();
            var validator = new CancelSubscriptionCommandValidator(context);
            var request = new CancelSubscriptionCommand
            {
                UserId = "sample-user",
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
                UserId = "non-existing-user",
                Platform = "DebugMessageService"
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.Contain(x => x.ErrorMessage == "The specified condition was not met for ''.");
        }
    }
}