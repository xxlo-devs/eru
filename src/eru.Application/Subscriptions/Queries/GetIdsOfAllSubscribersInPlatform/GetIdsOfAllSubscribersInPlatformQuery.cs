using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using MediatR;

namespace eru.Application.Subscriptions.Queries.GetIdsOfAllSubscribersInPlatform
{
    public class GetIdsOfAllSubscribersInPlatformQuery : IRequest<IEnumerable<string>>
    {
        public string Platform { get; set; }
    }

    public class GetIdsOfAllSubscribersInPlatformQueryHandler : IRequestHandler<GetIdsOfAllSubscribersInPlatformQuery, IEnumerable<string>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetIdsOfAllSubscribersInPlatformQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<IEnumerable<string>> Handle(GetIdsOfAllSubscribersInPlatformQuery request, CancellationToken cancellationToken)
            => Task.FromResult(_dbContext.Subscribers.Where(x => x.Platform == request.Platform).Select(x => x.Id).AsEnumerable());
    }
}
