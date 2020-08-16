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
    public class AppendClassCommandValidator : AbstractValidator<AppendClassCommand>
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

        private async Task<bool> DoesUserExist(AppendClassCommand command, CancellationToken cancellationToken) =>
            await _dbContext.Users.FindAsync(command.UserId, command.Platform) != null ? true : false;

        private async Task<bool> DoesClassExist(string @class, CancellationToken cancellationToken) => 
            await _dbContext.Classes.FindAsync(@class) != null ? true : false;

        private async Task<bool> IsOnRightStage(AppendClassCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FindAsync(command.UserId, command.Platform);

            if (user != null)
                if (user.Stage == Stage.GatheredLanguage) return true;

            return false;
        }
    }
}
