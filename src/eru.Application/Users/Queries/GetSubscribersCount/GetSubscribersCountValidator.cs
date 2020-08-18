using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Users.Queries.GetSubscribersCount
{
    public class GetSubscribersCountValidator : AbstractValidator<GetSubscribersCount>
    {
        private readonly IApplicationDbContext _context;
        public GetSubscribersCountValidator(IApplicationDbContext context)
        {
            _context = context;
            RuleFor(x => x.ClassName)
                .MustAsync(IsValidClassName);
        }

        private async Task<bool> IsValidClassName(string name, CancellationToken cancellationToken)
        {
            return name == null || await _context.Classes.AnyAsync(x => x.Name == name, cancellationToken);
        }
    }
}