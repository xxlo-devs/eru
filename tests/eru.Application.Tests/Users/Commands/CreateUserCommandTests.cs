using eru.Application.Users.Commands.CreateUser;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using FluentAssertions;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace eru.Application.Tests.Users.Commands
{
    public class CreateUserCommandTests
    {
        [Fact]
        public async Task ShouldCreateUserCorrectly()
        {
            var context = new FakeDbContext();

            var handler = new CreateUserCommandHandler(context);
            var request = new CreateUserCommand
            {
                Id = "new-user",
                Platform = "DebugMessageService",
                Class = "III c",
                PreferredLanguage = "en-US"
            };

            await handler.Handle(request, CancellationToken.None);

            context.Users.Should().ContainSingle(x =>
                x.Id == "new-user" & x.Platform == "DebugMessageService" & x.Class == "III c" & x.PreferredLanguage == "en-US");
        }

        [Fact]
        public async Task DoesValidatorAllowsCorrectRequest()
        {
            var context = new FakeDbContext();
            var validator = new CreateUserCommandValidator(context);

            var request = new CreateUserCommand
            {
                Id = "new-user",
                Platform = "DebugMessageService",
                Class = "III c",
                PreferredLanguage = "en-US"
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public async Task DoesValidatorPreventFromCreatingDuplicateAccount()
        {
            var context = new FakeDbContext();
            var validator = new CreateUserCommandValidator(context);
            var request = new CreateUserCommand
            {
                Id = "sample-user",
                Platform = "DebugMessageService",
                Class = "I a",
                PreferredLanguage = "en-US"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x => x.ErrorMessage == "The specified condition was not met for ''.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromCreatingAccountWithInvalidClass()
        {
            var context = new FakeDbContext();
            var validator = new CreateUserCommandValidator(context);
            var request = new CreateUserCommand
            {
                Id = "new-user",
                Platform = "DebugMessageService",
                Class = "Vz",
                PreferredLanguage = "en-US"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x => x.ErrorMessage == "The specified condition was not met for 'Class'.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromCreatingAccountWithInvalidLanguage()
        {
            var context = new FakeDbContext();
            var validator = new CreateUserCommandValidator(context);
            var request = new CreateUserCommand
            {
                Id = "new-user",
                Platform = "DebugMessageService",
                Class = "II b",
                PreferredLanguage = "nonexistinglanguage"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x => x.ErrorMessage == "The specified condition was not met for 'Preferred Language'.");
        }
    }
}
