using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Commands.CreateClass;
using FluentAssertions;
using FluentValidation;
using Xunit;

namespace eru.Application.Tests.Classes.Commands
{
    public class CreateClassCommandTests
    {
        public CreateClassCommandTests()
        {
            ValidatorOptions.Global.LanguageManager.Enabled = false;
        }
        [Fact]
        public async Task ShouldCreateClassCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new CreateClassCommandHandler(context);
            var request = new CreateClassCommand(3, "f");

            await handler.Handle(request, CancellationToken.None);

            context.Classes.Should().HaveCount(2).And.Contain(x => x.Year == 3 & x.Section == "f");
        }

        [Fact]
        public async Task DoesValidatorAllowCorrectCreateClassCommand()
        {
            var context = new FakeDbContext();
            var validator = new CreateClassCommandValidator(context);
            var request = new CreateClassCommand(3, "f");

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public async Task DoesValidatorPreventFromCreatingClassWithTooLongSectionName()
        {
            var context = new FakeDbContext();
            var validator = new CreateClassCommandValidator(context);
            var request = new CreateClassCommand(3, new string(Enumerable.Repeat('a',300).ToArray()));

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x=>x.ErrorCode == "MaximumLengthValidator");
        }

        [Fact]
        public async Task DoesValidatorPreventFromCreatingClassWithNoYearAndSection()
        {
            var context = new FakeDbContext();
            var validator = new CreateClassCommandValidator(context);
            var request = new CreateClassCommand(0, null);
            
            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2).And.Contain(x=>x.ErrorCode == "NotEmptyValidator");
        }

        [Fact]
        public async Task DoesValidatorPreventFromCreatingDuplicateClass()
        {
            var context = new FakeDbContext();
            var validator = new CreateClassCommandValidator(context);
            var request = new CreateClassCommand(1, "a");

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x=>x.ErrorMessage == "Mentioned class must be unique." && x.ErrorCode == "AsyncPredicateValidator");
        }

    }
}