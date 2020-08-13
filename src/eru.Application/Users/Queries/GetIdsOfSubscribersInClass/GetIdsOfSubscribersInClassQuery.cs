using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using eru.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Users.Queries.GetIdsOfSubscribersInClass
{
    public class GetIdsOfSubscribersInClassQuery : IRequest<IEnumerable<string>>
    {
        public string Class { get; set; }
        public Platform Platform { get; set; }
    }

    public class GetIdsOfSubscribersInClassQueryHandler : IRequestHandler<GetIdsOfSubscribersInClassQuery, IEnumerable<string>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetIdsOfSubscribersInClassQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<string>> Handle(GetIdsOfSubscribersInClassQuery request, CancellationToken cancellationToken) =>
            (from x in _dbContext.Users where x.Platform == request.Platform & x.Class == request.Class select x.Id).AsEnumerable();

    }
}
