using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Enums;
using FluentValidation;

namespace eru.Application.Users.Commands.AppendPreferredLanguage
{
    public class AppendPreferredLanguageCommandValidator : AbstractValidator<AppendPreferredLanguageCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        public AppendPreferredLanguageCommandValidator(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x)
                .MustAsync(DoesUserExist)
                .MustAsync(IsOnRightStage);
        }

        private async Task<bool> DoesUserExist(AppendPreferredLanguageCommand command,
            CancellationToken cancellationToken)
            => await _dbContext.Users.FindAsync(command.UserId, command.Platform) != null ? true : false;

        private async Task<bool> IsOnRightStage(AppendPreferredLanguageCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FindAsync(command.UserId, command.Platform);

            if (user != null)
                if (user.Stage == Stage.Created || user.Stage == Stage.Cancelled) return true;

            return false;
        }
    }
}
