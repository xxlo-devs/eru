using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Users.Commands.CancelSubscription
{
    public class CancelSubscriptionCommandValidator : AbstractValidator<CancelSubscriptionCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        public CancelSubscriptionCommandValidator(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x)
                .MustAsync(DoesUserExist).WithMessage("Mentioned user must already exist.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId cannot be empty.")
                .MaximumLength(255).WithMessage("UserId must have length up to 255 characters.");

            RuleFor(x => x.Platform)
                .NotEmpty().WithMessage("Platform cannot be empty.")
                .MaximumLength(255).WithMessage("Platform must have length up to 255 characters.");
        }

        private async Task<bool> DoesUserExist(CancelSubscriptionCommand command, CancellationToken cancellationToken) => 
            await _dbContext.Users.FindAsync(command.UserId, command.Platform) != null;
    }
}
