using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using MediatR;

namespace eru.Application.Subscriptions.Commands.CreateSubscription
{
    public class CreateSubscriptionCommand : IRequest
    {
        public CreateSubscriptionCommand(string id, string platform, string preferredLanguage, string @class)
        {
            Id = id;
            Platform = platform;
            PreferredLanguage = preferredLanguage;
            Class = @class;
        }
        public string Id { get; set; }
        public string Platform { get; set; }
        public string PreferredLanguage { get; set; }
        public string Class { get; set; }
    }

    public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, Unit>
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateSubscriptionCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(CreateSubscriptionCommand command, CancellationToken cancellationToken)
        {
            var subscriber = new Subscriber
            {
                Id = command.Id,
                Platform = command.Platform,
                Class = command.Class,
                PreferredLanguage = command.PreferredLanguage
            };

            await _dbContext.Subscribers.AddAsync(subscriber, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
