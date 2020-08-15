using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Enums;
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
                .MustAsync(DoesUserExist)
                .MustAsync(IsOnRightStage);
        }

        private async Task<bool> DoesUserExist(CancelSubscriptionCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Where(x => x.Id == command.UserId & x.Platform == command.Platform)
                .FirstOrDefaultAsync();

            if (user != null) return true;
            else return false;
        }

        public async Task<bool> IsOnRightStage(CancelSubscriptionCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Where(x => x.Id == command.UserId & x.Platform == command.Platform)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                if (user.Stage != Stage.Cancelled) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }
    }
}
