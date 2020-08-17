using System.Linq;
using System.Net;
using eru.Domain.Entity;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace eru.Application.Substitutions.Commands
{
    public class UploadSubstitutionsCommandValidator : AbstractValidator<UploadSubstitutionsCommand>
    {
        private readonly IConfiguration _configuration;

        public UploadSubstitutionsCommandValidator(IConfiguration configuration)
        {
            _configuration = configuration;

            RuleFor(x => x.IpAddress)
                .NotEmpty()
                .Must(IsIPAddressValid);
                
            RuleFor(x => x.Key)
                .NotEmpty()
                .Must(IsKeyValid);

            RuleFor(x => x.SubstitutionsPlan)
                .NotEmpty()
                .Must(IsPlanValid);
        }

        private bool IsKeyValid(string key)
        {
            return _configuration.GetValue<string>("UploadKey") == key;
        }

        private bool IsPlanValid(SubstitutionsPlan plan)
        {
            return plan?.Substitutions?.Any() == true;
        }

        private bool IsIPAddressValid(string address)
            => IPAddress.TryParse(address, out IPAddress ip);
    }
}