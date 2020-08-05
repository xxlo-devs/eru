using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Classes.Commands.RemoveClass
{
    public class RemoveClassValidator : AbstractValidator<RemoveClassCommand>
    {
        private readonly IApplicationDbContext _context;
        public RemoveClassValidator(IApplicationDbContext context)
        {
            _context = context;
            RuleFor(x => x.Name)
                .NotEmpty()
                .MustAsync(Exist);
        }

        private async Task<bool> Exist(string name, CancellationToken cancellationToken)
        {
            return await _context.Classes.AnyAsync(x => x.Name == name, cancellationToken: cancellationToken);
        }
    }
}