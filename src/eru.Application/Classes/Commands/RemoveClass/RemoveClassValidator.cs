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
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id cannot be empty.")
                .MustAsync(Exist).WithMessage("Mentioned class must already exist.");
        }

        private async Task<bool> Exist(string name, CancellationToken cancellationToken) 
            => await _context.Classes.AnyAsync(x => x.Id == name, cancellationToken: cancellationToken);
    }
}