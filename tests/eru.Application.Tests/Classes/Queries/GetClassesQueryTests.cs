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
                new ClassDto {Id = MockData.ExistingClassId, Year = 1, Section = "a"}
            };

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().HaveCount(1).And.BeEquivalentTo(expectedClasses);
        }
    }
}