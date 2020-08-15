using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Common.Interfaces;
using eru.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Classes.Queries.GetClassesFromGivenYear
{
    public class GetClassesFromGivenYearQuery : IRequest<IEnumerable<ClassDto>>
    {
        public Year Year { get; set; }
    }

    public class GetClassesFromGivenYearQueryHandler : IRequestHandler<GetClassesFromGivenYearQuery, IEnumerable<ClassDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetClassesFromGivenYearQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClassDto>> Handle(GetClassesFromGivenYearQuery request, CancellationToken cancellationToken) 
            => await _context.Classes
                .Where(x => x.Year == request.Year)
                .OrderBy(x => x.Name)
                .ProjectTo<ClassDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
    }
}
