using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using MediatR;

namespace eru.Application.Subscriptions.Queries.GetSubscriber
{
    public class GetSubscriberQuery : IRequest<Subscriber>
    {
        public GetSubscriberQuery(string id, string platform)
        {
            Id = id;
            Platform = platform;
        }

        public string Id { get; set; }
        public string Platform { get; set; }
    }

    public class GetSubscriberQueryHandler : IRequestHandler<GetSubscriberQuery, Subscriber>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetSubscriberQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Subscriber> Handle(GetSubscriberQuery request, CancellationToken cancellationToken)
            => await _dbContext.Subscribers.FindAsync(request.Id, request.Platform);
    }
}
