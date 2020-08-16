﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Users.Queries.GetIdsOfAllSubscribersInPlatform
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

        public async Task<IEnumerable<string>> Handle(GetIdsOfAllSubscribersInPlatformQuery request, CancellationToken cancellationToken)
            => _dbContext.Users.Where(x => x.Platform == request.Platform).Select(x => x.Id).AsEnumerable();
    }
}
