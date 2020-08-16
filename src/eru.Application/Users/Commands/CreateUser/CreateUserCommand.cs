using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using eru.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace eru.Application.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest
    {
        public string Id { get; set; }
        public string Platform { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Unit>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public CreateUserCommandHandler(IApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<Unit> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = command.Id,
                Platform = command.Platform,
                Class = string.Empty,
                PreferredLanguage = _configuration.GetValue<string>("DefaultLanguage"),
                Stage = Stage.Created
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
