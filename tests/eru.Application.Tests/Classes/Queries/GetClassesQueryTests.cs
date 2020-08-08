using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Common.Mappings;
using FluentAssertions;
using Xunit;

namespace eru.Application.Tests.Classes.Queries
{
    public class GetClassesQueryTests
    {
        [Fact]
        public async Task ShouldReturnAllClassesCorrectly()
        {
            await using var context =  new FakeDbContext();
            var mapper = new Mapper(new MapperConfiguration(configuration =>
            {
                configuration.AddProfile<MappingProfile>();
            }));
            var handler = new GetClassesQueryHandler(context, mapper);
            var request = new GetClassesQuery();
            var expectedClasses = new List<ClassDto>()
            {
                new ClassDto {Name = "język Angielski"},
                new ClassDto {Name = "język Polski"},
                new ClassDto {Name = "matematyka"}
            };

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().HaveCount(3).And.BeEquivalentTo(expectedClasses);
        }
    }
}