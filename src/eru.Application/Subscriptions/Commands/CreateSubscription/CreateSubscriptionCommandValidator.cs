using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using FluentValidation;

namespace eru.Application.Subscriptions.Commands.CreateSubscription
{
    public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateSubscriptionCommandValidator(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x)
                .MustAsync(IsSubscriberUnique).WithMessage("Mentioned subscriber must not exist.");

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id cannot be empty.")
                .MaximumLength(255).WithMessage("Id must have length up to 255 characters.");

            RuleFor(x => x.Platform)
                .NotEmpty().WithMessage("Platform cannot be empty.")
                .MaximumLength(255).WithMessage("Platform must have length up to 255 characters.");

            RuleFor(x => x.Class)
                .NotEmpty().WithMessage("Class cannot be empty.")
                .MaximumLength(255).WithMessage("Class must have length up to 255 characters.")
                .MustAsync(DoesClassExist).WithMessage("Mentioned class must already exist.");

            RuleFor(x => x.PreferredLanguage)
                .NotEmpty().WithMessage("PreferredLanguage cannot be empty.")
                .MaximumLength(255).WithMessage("PreferredLanguage must have length up to 255 characters")
                .Must(DoesLanguageExist).WithMessage("PreferredLanguage must be a valid iso language code.");
        }

        private async Task<bool> IsSubscriberUnique(CreateSubscriptionCommand command, CancellationToken cancellationToken) => 
            await _dbContext.Subscribers.FindAsync(command.Id, command.Platform) == null;

        private async Task<bool> DoesClassExist(string className, CancellationToken cancellationToken) =>
            await _dbContext.Classes.FindAsync(className) != null;

        private bool DoesLanguageExist(string lang)
        {
            try
            {
                _ = new CultureInfo(lang);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
