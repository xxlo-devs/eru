using eru.Application.Users.Commands.CreateUser;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using eru.Domain.Enums;
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
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("DefaultLanguage", "pl"),
                }).Build();
            
            var handler = new CreateUserCommandHandler(context, config);
            var request = new CreateUserCommand
            {
                Id = "90A17474-1410-4E00-9480-DDE01656F45B",
                Platform = "DebugMessageService"
            };

            await handler.Handle(request, CancellationToken.None);

            context.Users.Should().ContainSingle(x =>
                x.Id == "90A17474-1410-4E00-9480-DDE01656F45B" & x.Platform == "DebugMessageService" & x.Class.IsNullOrEmpty() & x.Stage == Stage.Created);
        }

        [Fact]
        public async Task DoesValidatorPreventFromCreatingDuplicateAccount()
        {
            var context = new FakeDbContext();
            var validator = new CreateUserCommandValidator(context);
            var request = new CreateUserCommand
            {
                Id = "380AE765-803D-4174-A370-1038B7D53CD6",
                Platform = "DebugMessageService"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x => x.ErrorMessage == "The specified condition was not met for ''.");
        }
    }
}
