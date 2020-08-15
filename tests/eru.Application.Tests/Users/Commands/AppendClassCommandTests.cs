using System.Threading;
using eru.Application.Users.Commands.AppendClass;
using System.Threading.Tasks;
using eru.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace eru.Application.Tests.Users.Commands
{
    public class AppendClassCommandTests
    {
        [Fact]
        public async Task ShouldAppendClassCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new AppendClassCommandHandler(context);
            var request = new AppendClassCommand
            {
                UserId = "195CC4D0-80F5-4745-86AC-7FCD3BAF209B",
                Platform = Platform.DebugMessageService,
                Class = "I a"
            };

            await handler.Handle(request, CancellationToken.None);

            context.Users.Should().Contain(x =>
                x.Id == "195CC4D0-80F5-4745-86AC-7FCD3BAF209B" & x.Platform == Platform.DebugMessageService &
                x.Year == Year.Freshman & x.Class == "I a" & x.Stage == Stage.GatheredClass);
        }

        [Fact]
        public async Task DoesValidatorAllowCorectAppendClassCommand()
        {
            var context = new FakeDbContext();
            var validator = new AppendClassCommandValidator(context);
            var request = new AppendClassCommand
            {
                UserId = "195CC4D0-80F5-4745-86AC-7FCD3BAF209B",
                Platform = Platform.DebugMessageService,
                Class = "I a"
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task DoesValidatorPreventFromAppendingNonExistingClass()
        {
            var context = new FakeDbContext();
            var validator = new AppendClassCommandValidator(context);
            var request = new AppendClassCommand
            {
                UserId = "195CC4D0-80F5-4745-86AC-7FCD3BAF209B",
                Platform = Platform.DebugMessageService,
                Class = "Vz"
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2).And
                .ContainSingle(x => x.ErrorMessage == "The specified condition was not met for 'Class'.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromAppendingClassToNonExistingUser()
        {
            var context = new FakeDbContext();
            var validator = new AppendClassCommandValidator(context);
            var request = new AppendClassCommand
            {
                UserId = "A97A1C73-5108-4CB7-8D39-408B824AC87B",
                Platform = Platform.DebugMessageService,
                Class = "I a"
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(3).And
                .Contain(x => x.ErrorMessage == "The specified condition was not met for ''.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromAppendingClassOnInvalidStage()
        {
            var context = new FakeDbContext();
            var validator = new AppendClassCommandValidator(context);
            var request = new AppendClassCommand
            {
                UserId = "98DFFEBA-BEB4-4D76-8C89-78857D7B7A2F",
                Platform = Platform.DebugMessageService,
                Class = "II b"
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2).And
                .Contain(x => x.ErrorMessage == "The specified condition was not met for ''.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromAppendingClassFromAntoherYear()
        {
            var context = new FakeDbContext();
            var validator = new AppendClassCommandValidator(context);
            var request = new AppendClassCommand
            {
                UserId = "195CC4D0-80F5-4745-86AC-7FCD3BAF209B",
                Platform = Platform.DebugMessageService,
                Class = "II b"
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And
                .ContainSingle(x => x.ErrorMessage == "The specified condition was not met for ''.");
        }
    }
}