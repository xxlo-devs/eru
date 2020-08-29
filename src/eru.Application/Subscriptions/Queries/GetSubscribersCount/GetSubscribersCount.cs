using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Subscriptions.Queries.GetSubscribersCount
{
    public class GetSubscribersCount : IRequest<int>
    {
        public GetSubscribersCount()
        {
            
        }

        public GetSubscribersCount(string classId)
        {
            ClassId = classId;
        }
        public string ClassId { get; set; }
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
            if (request.ClassId != null)
            {
                return await _context.Subscribers.Where(x => x.Class == request.ClassId).CountAsync(cancellationToken);
            }
            return await _context.Subscribers.CountAsync(cancellationToken);
        }
    }
}