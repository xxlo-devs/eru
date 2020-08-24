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
                Class = MockData.ExistingClassId,
                PreferredLanguage = "en-US"
            };

            await handler.Handle(request, CancellationToken.None);

            context.Users.Should().ContainSingle(x =>
                x.Id == "new-user" & x.Platform == "DebugMessageService" & x.Class == MockData.ExistingClassId & x.PreferredLanguage == "en-US");
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
                Class = MockData.ExistingClassId,
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
                Id = MockData.ExistingUserId,
                Platform = "DebugMessageService",
                Class = MockData.ExistingClassId,
                PreferredLanguage = "en-US"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x => x.ErrorCode == "AsyncPredicateValidator" && x.ErrorMessage == "Mentioned user must not exist.");
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
                Class = "invalid-class-id",
                PreferredLanguage = "en-US"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x => x.ErrorCode == "AsyncPredicateValidator" && x.ErrorMessage == "Mentioned class must already exist.");
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
                Class = MockData.ExistingClassId,
                PreferredLanguage = "nonexistinglanguage"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x => x.ErrorCode == "PredicateValidator" && x.ErrorMessage == "PreferredLanguage must be a valid iso language code.");
        }
    }
}
