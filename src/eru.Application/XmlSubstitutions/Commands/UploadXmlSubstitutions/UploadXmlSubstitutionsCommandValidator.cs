using System.IO;
using FluentValidation;

namespace eru.Application.XmlSubstitutions.Commands.UploadXmlSubstitutions
{
    public class UploadXmlSubstitutionsCommandValidator : AbstractValidator<UploadXmlSubstitutionsCommand>
    {
        public UploadXmlSubstitutionsCommandValidator()
        {
            RuleFor(x => x.FileStream)
                .NotEmpty()
                .Must(BeAtPositionZero)
                .Must(BeLongerThan0)
                .Must(BeAbleToBeRead);
        }
        
        private static bool BeAtPositionZero(Stream stream) => stream.Position == 0;

        private static bool BeAbleToBeRead(Stream stream) => stream.CanRead;

        private static bool BeLongerThan0(Stream stream) => stream.Length > 0;
    }
}