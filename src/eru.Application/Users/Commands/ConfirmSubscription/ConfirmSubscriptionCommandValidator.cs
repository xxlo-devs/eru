﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Users.Commands.ConfirmSubscription
{
    class ConfirmSubscriptionCommandValidator : AbstractValidator<ConfirmSubscriptionCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        public ConfirmSubscriptionCommandValidator(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x)
                .MustAsync(DoesUserExist)
                .MustAsync(IsOnRightStage);
        }

        private async Task<bool> DoesUserExist(ConfirmSubscriptionCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Where(x => x.Id == command.UserId & x.Platform == command.Platform)
                .FirstOrDefaultAsync();

            if (user != null) return true;
            else return false;
        }

        private async Task<bool> IsOnRightStage(ConfirmSubscriptionCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Where(x => x.Id == command.UserId & x.Platform == command.Platform)
                .FirstOrDefaultAsync();

            if (user.Stage == Stage.GatheredClass) return true;
            else return false;
        }
    }
}
