using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Xunit;
using eru.Application.Users.Commands.AppendYear;
using eru.Domain.Enums;
using FluentAssertions;
using Microsoft.VisualBasic;

namespace eru.Application.Tests.Users.Commands
{
    public class AppendYearCommandTest
    {
        [Fact]
        public async Task ShouldAppendYearCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new AppendYearCommandHandler(context);
            var request = new AppendYearCommand
            {
                UserId = "98DFFEBA-BEB4-4D76-8C89-78857D7B7A2F",
                Platform = Platform.DebugMessageService,
                Year = Year.Sophomore
            };

            await handler.Handle(request, CancellationToken.None);

            context.Users.Should().Contain(x =>
                x.Id == "98DFFEBA-BEB4-4D76-8C89-78857D7B7A2F" & x.Platform == Platform.DebugMessageService & x.Year ==
                Year.Sophomore & x.Class == String.Empty & x.Stage == Stage.GatheredYear);
        }

        [Fact]
        public async Task DoesValidatorAllowsCorrectAppendYearCommand()
        {
            var context = new FakeDbContext();
            var validator = new AppendYearCommandValidator(context);
            var request = new AppendYearCommand
            {
                UserId = "98DFFEBA-BEB4-4D76-8C89-78857D7B7A2F",
                Platform = Platform.DebugMessageService,
                Year = Year.Sophomore
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public async Task DoesValidatorPreventFromAppendingYearToNonExistingUser()
        {
            var context = new FakeDbContext();
            var validator = new AppendYearCommandValidator(context);
            var request = new AppendYearCommand
            {
                UserId = "FECEDC07-849C-4772-A3F7-9D7A5B97BBFE",
                Platform = Platform.DebugMessageService,
                Year = Year.Sophomore
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2).And.Contain(x => x.ErrorMessage == "The specified condition was not met for ''.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromAppendingYearOnInvalidStage()
        {
            var context = new FakeDbContext();
            var validator = new AppendYearCommandValidator(context);
            var request = new AppendYearCommand
            {
                UserId = "380AE765-803D-4174-A370-1038B7D53CD6",
                Platform = Platform.DebugMessageService,
                Year = Year.Sophomore
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.Contain(x => x.ErrorMessage == "The specified condition was not met for ''.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromAppendingInvalidYear()
        {
            var context = new FakeDbContext();
            var validator = new AppendYearCommandValidator(context);
            var request = new AppendYearCommand
            {
                UserId = "98DFFEBA-BEB4-4D76-8C89-78857D7B7A2F",
                Platform = Platform.DebugMessageService,
                Year = Year.NotSupplied
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x => x.ErrorMessage == "'Year' must not be empty.");
        }
    }
}