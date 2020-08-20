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
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(255).WithMessage("Name must have length up to 255 characters.")
                .MustAsync(IsUnique).WithMessage("Name must be unique.");
        }

        private async Task<bool> IsUnique(string name, CancellationToken cancellationToken) 
            => !await _context.Classes.AnyAsync(x => x.Name == name, cancellationToken);
    }
}