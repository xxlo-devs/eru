using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using MediatR;

namespace eru.Application.Users.Queries.GetUser
{
    public class GetUserQuery : IRequest<User>
    {
        public string UserId { get; set; }
        public string Platform { get; set; }
    }

    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, User>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetUserQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
            => await _dbContext.Users.FindAsync(request.UserId, request.Platform);
    }
}
