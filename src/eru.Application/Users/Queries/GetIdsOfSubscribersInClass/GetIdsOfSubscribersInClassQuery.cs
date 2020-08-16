using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using MediatR;

namespace eru.Application.Users.Queries.GetIdsOfSubscribersInClass
{
    public class GetIdsOfSubscribersInClassQuery : IRequest<IEnumerable<string>>
    {
        public string Class { get; set; }
        public string Platform { get; set; }
    }

    public class GetIdsOfSubscribersInClassQueryHandler : IRequestHandler<GetIdsOfSubscribersInClassQuery, IEnumerable<string>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetIdsOfSubscribersInClassQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<string>> Handle(GetIdsOfSubscribersInClassQuery request, CancellationToken cancellationToken)
            => _dbContext.Users.Where(x => x.Platform == request.Platform & x.Class == request.Class).Select(x => x.Id).AsEnumerable();
    }
}
