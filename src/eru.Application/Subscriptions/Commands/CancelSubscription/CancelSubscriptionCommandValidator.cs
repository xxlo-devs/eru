using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using FluentValidation;

namespace eru.Application.Subscriptions.Commands.CancelSubscription
{
    public class CancelSubscriptionCommandValidator : AbstractValidator<CancelSubscriptionCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        public CancelSubscriptionCommandValidator(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x)
                .MustAsync(DoesSubscriberExist).WithMessage("Mentioned subscriber must already exist.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId cannot be empty.")
                .MaximumLength(255).WithMessage("UserId must have length up to 255 characters.");

            RuleFor(x => x.Platform)
                .NotEmpty().WithMessage("Platform cannot be empty.")
                .MaximumLength(255).WithMessage("Platform must have length up to 255 characters.");
        }

        private async Task<bool> DoesSubscriberExist(CancelSubscriptionCommand command, CancellationToken cancellationToken) => 
            await _dbContext.Subscribers.FindAsync(command.UserId, command.Platform) != null;
    }
}
