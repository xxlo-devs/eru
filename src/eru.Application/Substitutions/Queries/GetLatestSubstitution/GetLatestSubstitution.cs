using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Substitutions.Queries.GetLatestSubstitution
{
    public class GetLatestSubstitution : IRequest<SubstitutionsRecord>
    {
        
    }
    
    public class GetLatestSubstitutionHandler : IRequestHandler<GetLatestSubstitution, SubstitutionsRecord>
    {
        private readonly IApplicationDbContext _context;

        public GetLatestSubstitutionHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SubstitutionsRecord> Handle(GetLatestSubstitution request, CancellationToken cancellationToken)
        {
            return await _context
                .SubstitutionsRecords
                .OrderBy(x => x.UploadDateTime)
                .Take(1)
                .Include(x=>x.Substitutions)
                    .ThenInclude(x=>x.Classes)
                .Select(x => new SubstitutionsRecord
                {
                    SubstitutionsDate = x.SubstitutionsDate,
                    UploadDateTime = x.UploadDateTime,
                    Substitutions = x.Substitutions
                        .OrderBy(y=>y.Lesson)
                        .ThenBy(y=>y.Subject)
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}