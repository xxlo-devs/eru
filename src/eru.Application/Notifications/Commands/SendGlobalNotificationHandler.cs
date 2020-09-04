using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using Hangfire;
using MediatR;

namespace eru.Application.Notifications.Commands
{
    public class SendGlobalNotificationHandler : IRequestHandler<SendGlobalNotification>
    {
        private readonly IEnumerable<IPlatformClient> _platformClients;
        private readonly IApplicationDbContext _dbContext;
        private readonly IBackgroundJobClient _hangfire;

        public SendGlobalNotificationHandler(IEnumerable<IPlatformClient> platformClients, IApplicationDbContext dbContext, IBackgroundJobClient hangfire)
        {
            _platformClients = platformClients;
            _dbContext = dbContext;
            _hangfire = hangfire;
        }

        public Task<Unit> Handle(SendGlobalNotification request, CancellationToken cancellationToken)
        {
            var subscribers = _dbContext.Subscribers.ToArray();
            var platformClients = _platformClients.ToDictionary(x => x.PlatformId);
            foreach (var subscriber in subscribers)
            {
                _hangfire.Enqueue(() =>
                    platformClients[subscriber.Platform].SendMessage(subscriber.Id, request.Content));
            }
            return Task.FromResult(Unit.Value);
        }
    }
}