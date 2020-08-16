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
using Microsoft.Extensions.Configuration;

namespace eru.Application.Users.Commands.CancelSubscription
{
    public class CancelSubscriptionCommand : IRequest
    {
        public string UserId { get; set; }
        public string Platform { get; set; }
    }

    public class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand, Unit>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public CancelSubscriptionCommandHandler(IApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<Unit> Handle(CancelSubscriptionCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FindAsync(command.UserId, command.Platform);

            user.Class = string.Empty;
            user.PreferredLanguage = _configuration.GetValue<string>("DefaultLanguage");
            user.Stage = Stage.Cancelled;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
