using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using eru.Domain.Enums;

namespace eru.Application.Users.Commands.AppendClass
{
    public class AppendClassCommand : IRequest
    {
        public string UserId { get; set; }
        public string Class { get; set; }
        public string Platform { get; set; }
    }

    public class AppendClassCommandHandler : IRequestHandler<AppendClassCommand, Unit>
    {
        private readonly IApplicationDbContext _dbContext;

        public AppendClassCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(AppendClassCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FindAsync(command.UserId, command.Platform);

            user.Class = command.Class;
            user.Stage = Stage.GatheredClass;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
