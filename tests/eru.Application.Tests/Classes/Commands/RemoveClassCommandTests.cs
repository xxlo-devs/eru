using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Commands.RemoveClass;
using eru.Domain.Entity;
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
            var request = new RemoveClassCommand(MockData.ExistingClassId);

            await handler.Handle(request, CancellationToken.None);

            (await context.Classes.ToArrayAsync()).Should().HaveCount(0).And.NotContain(new Class(3, "c"));
        }
        
        [Fact]
        public async Task DoesValidatorAllowCorrectRemoveClassCommand()
        {
            var context = new FakeDbContext();
            var validator = new RemoveClassValidator(context);
            var request = new RemoveClassCommand(MockData.ExistingClassId);

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public async Task DoesValidatorPreventFromRemovingClassWithNullIdGiven()
        {
            var context = new FakeDbContext();
            var validator = new RemoveClassValidator(context);
            var request = new RemoveClassCommand(null);

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2)
                .And
                .Contain(x=>x.ErrorCode == "NotEmptyValidator")
                .And
                .Contain(x=>x.ErrorCode == "AsyncPredicateValidator" && x.ErrorMessage == "Mentioned class must already exist.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromRemovingNonExistentClass()
        {
            var context = new FakeDbContext();
            var validator = new RemoveClassValidator(context);
            var request = new RemoveClassCommand("non-existent-class");

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x=>x.ErrorCode == "AsyncPredicateValidator" && x.ErrorMessage == "Mentioned class must already exist.");
        }
    }
}