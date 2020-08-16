﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using eru.Domain.Enums;
using MediatR;

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

        public CreateUserCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = command.Id,
                Platform = command.Platform,
                Class = string.Empty,
                Stage = Stage.Created
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
