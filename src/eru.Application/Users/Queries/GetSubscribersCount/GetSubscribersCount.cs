using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Users.Queries.GetSubscribersCount
{
    public class GetSubscribersCount : IRequest<int>
    {
        public GetSubscribersCount()
        {
            
        }

        public GetSubscribersCount(string className)
        {
            ClassName = className;
        }
        public string ClassName { get; set; } = null;
    }
    
    public class GetSubscribersCountHandler : IRequestHandler<GetSubscribersCount, int>
    {
        private readonly IApplicationDbContext _context;

        public GetSubscribersCountHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(GetSubscribersCount request, CancellationToken cancellationToken)
        {
            if (request.ClassName != null)
            {
                return await _context.Users.Where(x => x.Class == request.ClassName).CountAsync(cancellationToken);
            }
            return await _context.Users.CountAsync(cancellationToken);
        }
    }
}