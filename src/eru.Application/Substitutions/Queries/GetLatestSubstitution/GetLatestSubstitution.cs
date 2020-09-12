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
            var substitution = await _context
                .SubstitutionsRecords
                .OrderByDescending(x => x.UploadDateTime)
                .Take(1)
                .FirstOrDefaultAsync(cancellationToken);
            if (substitution is null)
                return null;
            return new SubstitutionsRecord
            {
                SubstitutionsDate = substitution.SubstitutionsDate,
                UploadDateTime = substitution.UploadDateTime,
                Substitutions = substitution.Substitutions
                    .OrderBy(y=>y.Lesson)
                    .ThenBy(y=>y.Subject)
            };
        }
    }
}