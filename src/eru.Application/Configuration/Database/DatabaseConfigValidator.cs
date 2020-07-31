using FluentValidation;

namespace eru.Application.Configuration.Database
{
    public class DatabaseConfigValidator : AbstractValidator<DatabaseConfig>
    {
        public DatabaseConfigValidator()
        {
            RuleFor(x => x.Database)
                .NotEmpty();
            RuleFor(x => x.Server)
                .NotEmpty();
            RuleFor(x => x.Username)
                .NotEmpty();
        }
    }
}