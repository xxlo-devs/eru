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
                .NotEmpty()
                .Must(IsIpAddressValid);
                
            RuleFor(x => x.Key)
                .NotEmpty()
                .Must(IsKeyValid);

            RuleFor(x => x.SubstitutionsPlan)
                .NotEmpty()
                .Must(IsPlanValid)
                .DependentRules(() =>
                {
                    RuleFor(x => x.SubstitutionsPlan)
                        .Must(AreAllClassesCorrect);
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