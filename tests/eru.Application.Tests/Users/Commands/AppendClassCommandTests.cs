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
                Class = "matematyka"
            };

            await handler.Handle(request, CancellationToken.None);

            context.Users.Should().Contain(x =>
                x.Id == "195CC4D0-80F5-4745-86AC-7FCD3BAF209B" & x.Platform == Platform.DebugMessageService &
                x.Year == Year.Freshman & x.Class == "matematyka" & x.Stage == Stage.GatheredClass);
        }
    }
}