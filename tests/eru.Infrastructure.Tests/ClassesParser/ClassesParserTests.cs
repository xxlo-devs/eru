using eru.Domain.Entity;
using FluentAssertions;
using Xunit;

namespace eru.Infrastructure.Tests.ClassesParser
{
    public class ClassesParserTests
    {
        [Fact]
        public async void CanParseEveryClassFormat()
        {
            var classNames = new[] {"1a", "2A", "3 a", "4 A", "Ia", "IA", "I b", "I B", "Ib1", "IB1"};
            var parser = new Infrastructure.ClassesParser.ClassesParser();

            var expectedClasses = new[]
            {
                new Class(1, "a"),
                new Class(2, "a"),
                new Class(3, "a"),
                new Class(4, "a"),
                new Class(1, "a"),
                new Class(1, "a"),
                new Class(1, "b"),
                new Class(1, "b"),
                new Class(1, "b1"),
                new Class(1, "b1")
            };

            var classes = await parser.Parse(classNames);

            classes.Should().BeEquivalentTo(expectedClasses);
        }

        [Fact]
        public async void CanParseEveryDigitYear()
        {
            var classNames = new[] {"1a", "2a", "3a", "4a", "5a", "6a", "7a", "8a", "9a", "10a", "11a", "12a"};
            var parser = new Infrastructure.ClassesParser.ClassesParser();

            var expectedClasses = new[]
            {
                new Class(1, "a"), 
                new Class(2, "a"), 
                new Class(3,"a"), 
                new Class(4, "a"), 
                new Class(5, "a"), 
                new Class(6, "a"), 
                new Class(7, "a"), 
                new Class(8, "a"), 
                new Class(9, "a"), 
                new Class(10, "a"), 
                new Class(11, "a"), 
                new Class(12, "a")
            };

            var result = await parser.Parse(classNames);
            
            result.Should().BeEquivalentTo(expectedClasses);
        }

        [Fact]
        public async void CanParseEveryRomanYear()
        {
            var classNames = new[] {"Ia", "IIa", "IIIa", "IVa", "Va", "VIa", "VIIa", "VIIIa", "IXa", "Xa", "XIa", "XIIa"};
            var parser = new Infrastructure.ClassesParser.ClassesParser();
            
            var expectedClasses = new[]
            {
                new Class(1, "a"), 
                new Class(2, "a"), 
                new Class(3,"a"), 
                new Class(4, "a"), 
                new Class(5, "a"), 
                new Class(6, "a"), 
                new Class(7, "a"), 
                new Class(8, "a"), 
                new Class(9, "a"), 
                new Class(10, "a"), 
                new Class(11, "a"), 
                new Class(12, "a")
            };

            var result = await parser.Parse(classNames);

            result.Should().BeEquivalentTo(expectedClasses);
        }
    }
}