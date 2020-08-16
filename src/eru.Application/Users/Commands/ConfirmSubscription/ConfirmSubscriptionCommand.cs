using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Users.Commands.ConfirmSubscription
{
    public class ConfirmSubscriptionCommand : IRequest
    {
        public string UserId { get; set; }
        public string Platform { get; set; }
    }

    public class ConfirmSubscriptionCommandHandler : IRequestHandler<ConfirmSubscriptionCommand, Unit>
    {
        private readonly IApplicationDbContext _dbContext;

        public ConfirmSubscriptionCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(ConfirmSubscriptionCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FindAsync(command.UserId, command.Platform);

            user.Stage = Stage.Subscribed;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
