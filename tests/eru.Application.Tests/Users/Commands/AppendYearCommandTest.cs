using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Xunit;
using eru.Application.Users.Commands.AppendYear;
using eru.Domain.Enums;
using FluentAssertions;

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
    }
}