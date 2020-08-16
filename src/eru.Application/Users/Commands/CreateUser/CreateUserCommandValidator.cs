using System;
using System.Collections.Generic;
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
                .MustAsync(IsIdUnique);

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
                .MaximumLength(255);
        }

        private async Task<bool> IsIdUnique(CreateUserCommand command, CancellationToken cancellationToken) => 
            await _dbContext.Users.FindAsync(command.Id, command.Platform) != null ? false : true;

        private async Task<bool> DoesClassExist(string className, CancellationToken cancellationToken) =>
            await _dbContext.Classes.FindAsync(className) != null ? true : false;
    }
}
