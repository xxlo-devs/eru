using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Users.Commands.AppendPreferredLanguage;
using eru.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace eru.Application.Tests.Users.Commands
{
    public class AppendPreferredLanguageCommandTests
    {
        [Fact]
        public async Task ShouldAppendPrefferedLanguageCorrectly()
        {
            var context = new FakeDbContext();

            var handler = new AppendPrefferedLanguageCommandHandler(context);
            var request = new AppendPreferredLanguageCommand
            {
                UserId = "98DFFEBA-BEB4-4D76-8C89-78857D7B7A2F",
                Platform = "DebugMessageService",
                PrefferedLanguage = "en-US"
            };

            await handler.Handle(request, CancellationToken.None);

            context.Users.Should().ContainSingle(x =>
                x.Id == "98DFFEBA-BEB4-4D76-8C89-78857D7B7A2F" & x.Platform == "DebugMessageService" &
                x.Class == String.Empty & x.Stage == Stage.GatheredLanguage & x.PreferredLanguage == "en-US");
        }

        [Fact]
        public async Task DoesValidatorAllowCorrectRequest()
        {
            var context = new FakeDbContext();
            var validator = new AppendPreferredLanguageCommandValidator(context);

            var request = new AppendPreferredLanguageCommand
            {
                UserId = "98DFFEBA-BEB4-4D76-8C89-78857D7B7A2F",
                Platform = "DebugMessageService",
                PrefferedLanguage = "en-US"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public async Task DoesValidatorPreventFromAppendingLanguageToNonExistingUser()
        {
            var context = new FakeDbContext();
            var validator = new AppendPreferredLanguageCommandValidator(context);

            var request = new AppendPreferredLanguageCommand
            {
                UserId = "E47D99A1-18C0-40F7-8222-1AF30E6FBD5D",
                Platform = "DebugMessageService",
                PrefferedLanguage = "en-US"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
        }

        [Fact]
        public async Task DoesValidatorPreventFromAppendingLanguageOnInvalidStage()
        {
            var context = new FakeDbContext();
            var validator = new AppendPreferredLanguageCommandValidator(context);

            var request = new AppendPreferredLanguageCommand
            {
                UserId = "380AE765-803D-4174-A370-1038B7D53CD6",
                Platform = "DebugMessageService",
                PrefferedLanguage = "en-US"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
        }
    }
}
