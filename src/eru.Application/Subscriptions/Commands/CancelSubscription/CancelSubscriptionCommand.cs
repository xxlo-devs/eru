using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using MediatR;

namespace eru.Application.Subscriptions.Commands.CancelSubscription
{
    public class CancelSubscriptionCommand : IRequest
    {
        public CancelSubscriptionCommand(string userId, string platform)
        {
            UserId = userId;
            Platform = platform;
        }
        public string UserId { get; set; }
        public string Platform { get; set; }
    }

    public class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand, Unit>
    {
        private readonly IApplicationDbContext _dbContext;

        public CancelSubscriptionCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(CancelSubscriptionCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Subscribers.FindAsync(command.UserId, command.Platform);

            _dbContext.Subscribers.Remove(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
