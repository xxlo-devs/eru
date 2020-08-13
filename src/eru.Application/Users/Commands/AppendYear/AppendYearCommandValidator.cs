using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Users.Commands.AppendYear
{
    class AppendYearCommandValidator : AbstractValidator<AppendYearCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        public AppendYearCommandValidator(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x)
                .MustAsync(DoesUserExist);
        }

        private async Task<bool> DoesUserExist(AppendYearCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Where(x => x.Id == command.UserId & x.Platform == command.Platform)
                .FirstOrDefaultAsync();

            if (user != null) return true;
            else return false;
        }
    }
}
