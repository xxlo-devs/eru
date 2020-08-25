using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using MediatR;

namespace eru.Application.Subscriptions.Queries.GetSubscriber
{
    public class GetUserQuery : IRequest<Subscriber>
    {
        public string UserId { get; set; }
        public string Platform { get; set; }
    }

    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Subscriber>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetUserQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Subscriber> Handle(GetUserQuery request, CancellationToken cancellationToken)
            => await _dbContext.Subscribers.FindAsync(request.UserId, request.Platform);
    }
}
