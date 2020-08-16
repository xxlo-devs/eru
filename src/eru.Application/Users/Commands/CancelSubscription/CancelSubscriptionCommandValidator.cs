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
                .NotEmpty()
                .MustAsync(DoesUserExist);

            RuleFor(x => x.UserId)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.Platform)
                .NotEmpty()
                .MaximumLength(255);
        }

        private async Task<bool> DoesUserExist(CancelSubscriptionCommand command, CancellationToken cancellationToken) => 
            await _dbContext.Users.FindAsync(command.UserId, command.Platform) != null ? true : false;
    }
}
