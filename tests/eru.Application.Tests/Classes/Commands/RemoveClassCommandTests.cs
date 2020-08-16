using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Commands.CreateClass;
using eru.Application.Classes.Commands.RemoveClass;
using eru.Domain.Entity;
using eru.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace eru.Application.Tests.Classes.Commands
{
    public class RemoveClassCommandTests
    {
        [Fact]
        public async Task ShouldRemoveClassCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new RemoveClassCommandHandler(context);
            var request = new RemoveClassCommand()
            {
                Name = "III c"
            };

            await handler.Handle(request, CancellationToken.None);

            (await context.Classes.ToArrayAsync()).Should().HaveCount(2).And.NotContain(new Class("III c"));
        }
        
        [Fact]
        public async Task DoesValidatorAllowCorrectRemoveClassCommand()
        {
            var context = new FakeDbContext();
            var validator = new RemoveClassValidator(context);
            var request = new RemoveClassCommand
            {
                Name = "III c"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public async Task DoesValidatorPreventFromRemovingClassWithNoNameGiven()
        {
            var context = new FakeDbContext();
            var validator = new RemoveClassValidator(context);
            var request = new RemoveClassCommand();

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2)
                .And
                .Contain(x=>x.ErrorMessage == "'Name' must not be empty.")
                .And
                .Contain(x=>x.ErrorMessage == "The specified condition was not met for 'Name'.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromRemovingNonExistentClass()
        {
            var context = new FakeDbContext();
            var validator = new RemoveClassValidator(context);
            var request = new RemoveClassCommand
            {
                Name = "II a"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x=>x.ErrorMessage == "The specified condition was not met for 'Name'.");
        }
    }
}