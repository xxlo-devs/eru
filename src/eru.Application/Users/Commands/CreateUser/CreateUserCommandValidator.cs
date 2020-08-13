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
                .MustAsync(IsIdUnique);
        }

        private async Task<bool> IsIdUnique(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var user = _dbContext.Users.FindAsync(command.Id, command.Platform);

            if (user == null) return true;
            else return false;
        }
    }
}
