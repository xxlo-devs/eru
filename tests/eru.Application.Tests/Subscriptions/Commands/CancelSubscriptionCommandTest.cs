using System.Threading;
using System.Threading.Tasks;
using eru.Application.Subscriptions.Commands.CancelSubscription;
using FluentAssertions;
using FluentValidation;
using Xunit;

namespace eru.Application.Tests.Subscriptions.Commands
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
            var request = new CancelSubscriptionCommand(MockData.ExistingSubscriberId, "DebugMessageService");

            await handler.Handle(request, CancellationToken.None);

            context.Subscribers.Should().NotContain(x => x.Id == MockData.ExistingSubscriberId & x.Platform == "DebugMessageService");
        }

        [Fact]
        public async Task DoesValidatorAllowCorrectCancelSubscriptionCommand()
        {
            var context = new FakeDbContext();
            var validator = new CancelSubscriptionCommandValidator(context);
            var request = new CancelSubscriptionCommand(MockData.ExistingSubscriberId, "DebugMessageService");

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public async Task DoesValidatorPreventFromCancellingNonExistingSubscription()
        {
            var context = new FakeDbContext();
            var validator = new CancelSubscriptionCommandValidator(context);
            var request = new CancelSubscriptionCommand("non-existing-subscriber", "DebugMessageService");

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.Contain(x=>x.ErrorCode == "AsyncPredicateValidator" && x.ErrorMessage == "Mentioned subscriber must already exist.");
        }
    }
}