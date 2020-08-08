using System.Collections.Generic;
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
        {
            return await _context.Classes
                .OrderBy(x => x.Name)
                .ProjectTo<ClassDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}