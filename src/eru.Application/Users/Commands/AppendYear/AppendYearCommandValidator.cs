using System;
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

namespace eru.Application.Users.Commands.AppendYear
{
    public class AppendYearCommandValidator : AbstractValidator<AppendYearCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        public AppendYearCommandValidator(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x)
                .MustAsync(DoesUserExist)
                .MustAsync(IsOnValidStage);

            RuleFor(x => x.Year)
                .NotEmpty();
        }

        private async Task<bool> DoesUserExist(AppendYearCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Where(x => x.Id == command.UserId & x.Platform == command.Platform)
                .FirstOrDefaultAsync();

            if (user != null) return true;
            else return false;
        }

        private async Task<bool> IsOnValidStage(AppendYearCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Where(x => x.Id == command.UserId & x.Platform == command.Platform)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                if (user.Stage == Stage.Created || user.Stage == Stage.Cancelled) return true;
                else return false;
            }
            else return false;
        }
    }
}
