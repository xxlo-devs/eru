using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Commands.CreateClass;
using FluentAssertions;
using Xunit;

namespace eru.Application.IntegrationTests.Classes.Commands
{
    public class CreateClassCommandTests
    {
        [Fact]
        public async Task ShouldCreateClassCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new CreateClassCommandHandler(context);
            var request = new CreateClassCommand
            {
                Name = "informatyka"
            };

            await handler.Handle(request, CancellationToken.None);

            context.Classes.Should().HaveCount(4).And.Contain(x => x.Name == "informatyka");
        }
    }
}