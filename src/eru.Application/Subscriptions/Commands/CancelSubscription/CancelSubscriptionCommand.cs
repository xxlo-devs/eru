using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using MediatR;

namespace eru.Application.Subscriptions.Commands.CancelSubscription
{
    public class CancelSubscriptionCommand : IRequest
    {
        public CancelSubscriptionCommand(string id, string platform)
        {
            Id = id;
            Platform = platform;
        }
        public string Id { get; set; }
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
            var subscriber = await _dbContext.Subscribers.FindAsync(command.Id, command.Platform);

            _dbContext.Subscribers.Remove(subscriber);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
