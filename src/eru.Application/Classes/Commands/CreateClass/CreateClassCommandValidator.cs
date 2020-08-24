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

            RuleFor(x => x)
                .MustAsync(IsUnique).WithMessage("Mentioned class must be unique.");

            RuleFor(x => x.Year)
                .NotEmpty().WithMessage("Year cannot be empty.")
                .Must(IsYearValid).WithMessage("Year must be between 0 and 12.");

            RuleFor(x => x.Section)
                .NotEmpty().WithMessage("Section cannot be empty.")
                .MaximumLength(255).WithMessage("Section must have length up to 255 characters");
        }

        private async Task<bool> IsUnique(CreateClassCommand command, CancellationToken cancellationToken) 
            => !await _context.Classes.AnyAsync(x => x.Year == command.Year & x.Section == command.Section, cancellationToken);
        
        private bool IsYearValid(int year) 
            => year >= 0 & year <= 12;
    }
}