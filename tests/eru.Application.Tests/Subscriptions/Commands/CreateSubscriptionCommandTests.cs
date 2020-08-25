using System.Threading;
using System.Threading.Tasks;
using eru.Application.Subscriptions.Commands.CreateSubscription;
using FluentAssertions;
using Xunit;

namespace eru.Application.Tests.Subscriptions.Commands
{
    public class CreateSupscriptionsCommandTests
    {
        [Fact]
        public async Task ShouldCreateSubscriptionCorrectly()
        {
            var context = new FakeDbContext();

            var handler = new CreateSubscriptionCommandHandler(context);
            var request = new CreateSubscriptionCommand
            {
                Id = "new-user",
                Platform = "DebugMessageService",
                Class = MockData.ExistingClassId,
                PreferredLanguage = "en-US"
            };

            await handler.Handle(request, CancellationToken.None);

            context.Subscribers.Should().ContainSingle(x =>
                x.Id == "new-user" & x.Platform == "DebugMessageService" & x.Class == MockData.ExistingClassId & x.PreferredLanguage == "en-US");
        }

        [Fact]
        public async Task DoesValidatorAllowsCorrectRequest()
        {
            var context = new FakeDbContext();
            var validator = new CreateSubscriptionCommandValidator(context);

            var request = new CreateSubscriptionCommand
            {
                Id = "new-user",
                Platform = "DebugMessageService",
                Class = MockData.ExistingClassId,
                PreferredLanguage = "en-US"
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public async Task DoesValidatorPreventFromCreatingDuplicateSubscription()
        {
            var context = new FakeDbContext();
            var validator = new CreateSubscriptionCommandValidator(context);
            var request = new CreateSubscriptionCommand
            {
                Id = MockData.ExistingUserId,
                Platform = "DebugMessageService",
                Class = MockData.ExistingClassId,
                PreferredLanguage = "en-US"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x => x.ErrorCode == "AsyncPredicateValidator" && x.ErrorMessage == "Mentioned subscriber must not exist.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromCreatingAccountWithNonExistingClass()
        {
            var context = new FakeDbContext();
            var validator = new CreateSubscriptionCommandValidator(context);
            var request = new CreateSubscriptionCommand
            {
                Id = "new-user",
                Platform = "DebugMessageService",
                Class = "invalid-class-id",
                PreferredLanguage = "en-US"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x => x.ErrorCode == "AsyncPredicateValidator" && x.ErrorMessage == "Mentioned class must already exist.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromCreatingAccountWithInvalidLanguage()
        {
            var context = new FakeDbContext();
            var validator = new CreateSubscriptionCommandValidator(context);
            var request = new CreateSubscriptionCommand
            {
                Id = "new-user",
                Platform = "DebugMessageService",
                Class = MockData.ExistingClassId,
                PreferredLanguage = "nonexistinglanguage"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x => x.ErrorCode == "PredicateValidator" && x.ErrorMessage == "PreferredLanguage must be a valid iso language code.");
        }
    }
}
