using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Commands.RemoveClass;
using eru.Domain.Entity;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace eru.Application.IntegrationTests.Classes.Commands
{
    public class RemoveClassCommandTests
    {
        [Fact]
        public async Task ShouldRemoveClassCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new RemoveClassCommandHandler(context);
            var request = new RemoveClassCommand()
            {
                Name = "matematyka"
            };

            await handler.Handle(request, CancellationToken.None);

            (await context.Classes.ToArrayAsync()).Should().HaveCount(2).And.NotContain(new Class("matematyka"));
        }
    }
}