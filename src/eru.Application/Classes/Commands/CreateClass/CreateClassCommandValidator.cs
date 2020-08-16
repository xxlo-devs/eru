using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Classes.Commands.CreateClass
{
    public class CreateClassCommandValidator : AbstractValidator<CreateClassCommand>
    {
        private readonly IApplicationDbContext _context;
        public CreateClassCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(255)
                .MustAsync(IsUnique);
        }

        private async Task<bool> IsUnique(string name, CancellationToken cancellationToken) 
            => !await _context.Classes.AnyAsync(x => x.Name == name, cancellationToken);
    }
}