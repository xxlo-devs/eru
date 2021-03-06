﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using eru.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Classes.Queries.GetClasses
{
    public class GetClassesQuery : IRequest<IEnumerable<ClassDto>>
    {
        
    }
    
    public class GetClassesQueryHandler : IRequestHandler<GetClassesQuery, IEnumerable<ClassDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetClassesQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClassDto>> Handle(GetClassesQuery request, CancellationToken cancellationToken) 
            => await _context.Classes
                .ProjectTo<ClassDto>(_mapper.ConfigurationProvider)
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Section)
                .ToListAsync(cancellationToken);
    }
}