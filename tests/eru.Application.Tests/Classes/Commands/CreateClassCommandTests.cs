using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Commands.CreateClass;
using eru.Domain.Enums;
using FluentAssertions;
using FluentValidation.Results;
using Xunit;

namespace eru.Application.Tests.Classes.Commands
{
    public class CreateClassCommandTests
    {
        [Fact]
        public async Task ShouldCreateClassCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new CreateClassCommandHandler(context);
            var request = new CreateClassCommand
            {
                Name = "III f",
                Year = Year.Junior
            };

            await handler.Handle(request, CancellationToken.None);

            context.Classes.Should().HaveCount(4).And.Contain(x => x.Name == "III f" & x.Year == Year.Junior);
        }

        [Fact]
        public async Task DoesValidatorAllowCorrectCreateClassCommand()
        {
            var context = new FakeDbContext();
            var validator = new CreateClassCommandValidator(context);
            var request = new CreateClassCommand
            {
                Name = "III f",
                Year = Year.Junior
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public async Task DoesValidatorPreventFromCreatingClassWithTooLongName()
        {
            var context = new FakeDbContext();
            var validator = new CreateClassCommandValidator(context);
            var request = new CreateClassCommand
            {
                Name = new string(Enumerable.Repeat('a',300).ToArray()), 
                Year = Year.Freshman
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x=>x.ErrorMessage == "The length of 'Name' must be 255 characters or fewer. You entered 300 characters.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromCreatingClassWithNoName()
        {
            var context = new FakeDbContext();
            var validator = new CreateClassCommandValidator(context);
            var request = new CreateClassCommand
            {
                Year = Year.Freshman
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x=>x.ErrorMessage == "'Name' must not be empty.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromCreatingClassWithSameNameAsPreviouslyCreatedClass()
        {
            var context = new FakeDbContext();
            var validator = new CreateClassCommandValidator(context);
            var request = new CreateClassCommand
            {
                Name = "II b",
                Year = Year.Sophomore
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x=>x.ErrorMessage == "The specified condition was not met for 'Name'.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromCreatingClassWithNoYear()
        {
            var context = new FakeDbContext();
            var validator = new CreateClassCommandValidator(context);
            var request = new CreateClassCommand
            {
                Name = "III d"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x=>x.ErrorMessage == "'Year' must not be empty.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromCreatingClassWithInvalidYear()
        {
            var context = new FakeDbContext();
            var validator = new CreateClassCommandValidator(context);
            var request = new CreateClassCommand
            {
                Name = "III d",
                Year = Year.NotSupplied
            };

            var result = await validator.ValidateAsync(request);

            result.Errors.Should().HaveCount(1).And.ContainSingle(x=>x.ErrorMessage == "'Year' must not be empty.");
        }
    }
}