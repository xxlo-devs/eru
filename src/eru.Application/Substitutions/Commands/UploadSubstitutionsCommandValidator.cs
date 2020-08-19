using System.Linq;
using System.Net;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace eru.Application.Substitutions.Commands
{
    public class UploadSubstitutionsCommandValidator : AbstractValidator<UploadSubstitutionsCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly IApplicationDbContext _context;

        public UploadSubstitutionsCommandValidator(IConfiguration configuration, IApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;

            RuleFor(x => x.IpAddress)
                .NotEmpty().WithMessage("IpAddress cannot be empty.")
                .Must(IsIpAddressValid).WithMessage("IpAddress must be a valid ip address.");
                
            RuleFor(x => x.Key)
                .NotEmpty().WithMessage("Key cannot be empty.")
                .Must(IsKeyValid).WithMessage("Key must be a correct key from configuration.");

            RuleFor(x => x.SubstitutionsPlan)
                .NotEmpty().WithMessage("SubstitutionsPlan cannot be empty.")
                .Must(IsPlanValid).WithMessage("SubstitutionsPlan must have at least one substitution.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.SubstitutionsPlan)
                        .Must(AreAllClassesCorrect).WithMessage("All classes mentioned in SubstitutionsPlan must be present in database.");
                });
        }

        private bool IsKeyValid(string key)
        {
            return _configuration.GetValue<string>("UploadKey") == key;
        }

        private bool IsPlanValid(SubstitutionsPlan plan)
        {
            return plan?.Substitutions?.Any() == true;
        }

        private bool AreAllClassesCorrect(SubstitutionsPlan plan)
        {
            return plan.Substitutions
                .SelectMany(x => x.Classes)
                .All(x => _context.Classes.Contains(x));
        }

        private bool IsIpAddressValid(string address)
            => IPAddress.TryParse(address, out _);
    }
}