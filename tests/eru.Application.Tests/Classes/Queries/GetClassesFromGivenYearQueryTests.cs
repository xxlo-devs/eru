using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Classes.Queries.GetClassesFromGivenYear;
using eru.Application.Common.Mappings;
using eru.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace eru.Application.Tests.Classes.Queries
{
    public class GetClassesFromGivenYearQueryTests
    {
        [Fact]
        public async Task ShouldReturnAllClassesCorrectly()
        {
            var context =  new FakeDbContext();
            var mapper = new Mapper(new MapperConfiguration(configuration =>
            {
                configuration.AddProfile<MappingProfile>();
            }));
            var handler = new GetClassesFromGivenYearQueryHandler(context, mapper);
            var request = new GetClassesFromGivenYearQuery
            {
                Year = Year.Sophomore
            };

            var expectedClasses = new List<ClassDto>()
            {
                new ClassDto {Name = "II b", Year = Year.Sophomore}
            };

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().HaveCount(1).And.BeEquivalentTo(expectedClasses);
        }
    }
}