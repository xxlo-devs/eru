using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateUserCommandValidator(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x)
                .NotEmpty()
                .MustAsync(IsUserUnique);

            RuleFor(x => x.Id)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.Platform)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.Class)
                .NotEmpty()
                .MaximumLength(255)
                .MustAsync(DoesClassExist);

            RuleFor(x => x.PreferredLanguage)
                .NotEmpty()
                .MaximumLength(255)
                .MustAsync(DoesLanguageExist);
        }

        private async Task<bool> IsUserUnique(CreateUserCommand command, CancellationToken cancellationToken) => 
            await _dbContext.Users.FindAsync(command.Id, command.Platform) != null ? false : true;

        private async Task<bool> DoesClassExist(string className, CancellationToken cancellationToken) =>
            await _dbContext.Classes.FindAsync(className) != null ? true : false;

        private async Task<bool> DoesLanguageExist(string lang, CancellationToken cancellationToken)
        {
            try
            {
                var cultureInfo = new CultureInfo(lang);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
