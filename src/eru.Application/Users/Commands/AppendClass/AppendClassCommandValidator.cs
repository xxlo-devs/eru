using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Users.Commands.AppendClass
{
    class AppendClassCommandValidator : AbstractValidator<AppendClassCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        public AppendClassCommandValidator(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x)
                .MustAsync(DoesUserExist)
                .MustAsync(IsOnRightStage);

            RuleFor(x => x.Class)
                .MustAsync(DoesClassExist);

        }

        private async Task<bool> DoesUserExist(AppendClassCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Where(x => x.Id == command.UserId & x.Platform == command.Platform)
                .FirstOrDefaultAsync();

            if (user != null) return true;
            else return false; 
        }

        private async Task<bool> IsOnRightStage(AppendClassCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Where(x => x.Id == command.UserId & x.Platform == command.Platform)
                .FirstOrDefaultAsync();

            if (user.Stage == Stage.GatheredYear) return true;
            else return false;
        }

        private async Task<bool> DoesClassExist(string @class, CancellationToken cancellationToken)
        {
             var obj = await _dbContext.Classes.FindAsync(@class);

             if (obj.Name != null) return true;
             else return false;
        }

    }
}
