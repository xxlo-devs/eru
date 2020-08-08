using FluentValidation;

namespace eru.Application.Tests.Common.Behaviours
{
    public class SampleValidator : AbstractValidator<SampleRequest>
    {
        public SampleValidator()
        {
            RuleFor(x => x.Version)
                .NotEmpty();
        }
    }
}