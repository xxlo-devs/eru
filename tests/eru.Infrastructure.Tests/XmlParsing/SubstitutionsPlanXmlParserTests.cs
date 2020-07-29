using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using eru.Application.Common.Exceptions;
using eru.Domain.Entity;
using eru.Infrastructure.XmlParsing;
using FluentAssertions;
using Xunit;

namespace eru.Infrastructure.Tests.XmlParsing
{
    public class SubstitutionsPlanXmlParserTests
    {
        [Fact]
        public async Task ParsesXmlCorrectly()
        {
            var stream = new MemoryStream(Encoding.Default.GetBytes(
                "<?xml version=\"1.0\" encoding=\"windows-1250\"?>\r\n<substitutions>\r\n   <date day=\"17\" month=\"2\" year=\"2020\">\r\n      <subst absent=\"Magdalena Sikora\" lesson=\"0\" subject=\"J\u0119zyk francuski\" forms=\"II c\" groups=\"fra\" cancelled=\"1\" note=\"\" room=\"304\"/>\r\n      <subst absent=\"Dorota Rutkowska\" lesson=\"1\" subject=\"J\u0119zyk niemiecki\" forms=\"II b,II a\" groups=\"niem2,niem2\" cancelled=\"1\" note=\"\" room=\"206\"/>\r\n      <subst absent=\"Magdalena Sikora\" lesson=\"1\" subject=\"J\u0119zyk francuski\" forms=\"II d\" groups=\"fra\" substituting=\"Biblioteka\" subst_type=\"biblioteka\" note=\"biblioteka\" room=\"304\"/>\r\n      <subst absent=\"Dorota Rutkowska\" lesson=\"2\" subject=\"J\u0119zyk niemiecki\" forms=\"Ia2\" groups=\"niem\" substituting=\"Emil Cie\u015Blak\" subst_type=\"p\u0142atne\" note=\"z\" room=\"208\"/>\r\n      <subst absent=\"Magdalena Sikora\" lesson=\"2\" subject=\"J\u0119zyk francuski\" forms=\"Ic2\" groups=\"fra\" substituting=\"Biblioteka\" subst_type=\"biblioteka\" note=\"biblioteka\" room=\"304\"/>\r\n      <subst absent=\"Dorota Rutkowska\" lesson=\"3\" subject=\"J\u0119zyk niemiecki\" forms=\"Id1\" groups=\"niem\" substituting=\"Marcin Dahm\" subst_type=\"p\u0142atne\" note=\"z\" room=\"300\"/>\r\n      <subst absent=\"Magdalena Sikora\" lesson=\"5\" subject=\"Godzina z wychowawc\u0105\" forms=\"II c\" groups=\"Ca\u0142a klasa\" substituting=\"Beata Pleszewska\" subst_type=\"p\u0142atne\" note=\"z\" room=\"304\"/>\r\n      <subst absent=\"Magdalena Sikora\" lesson=\"6\" subject=\"J\u0119zyk francuski\" forms=\"III c,III d\" groups=\"fra 2,fra 2\" substituting=\"Renata Dombrowska\" subst_type=\"ca\u0142o\u015B\u0107\" note=\"ca\u0142o\u015B\u0107\" room=\"114\"/>\r\n      <subst absent=\"Magdalena Sikora\" lesson=\"7\" subject=\"J\u0119zyk francuski\" forms=\"II a,II b\" groups=\"fra,fra\" cancelled=\"1\" note=\"\" room=\"304\"/>\r\n   </date>\r\n</substitutions>"));
            var expected = new SubstitutionsPlan
            {
                Date = new DateTime(2020, 2, 17),
                Substitutions = new[]
                {
                    new Substitution
                    {
                        Teacher = "Magdalena Sikora",
                        Lesson = 0,
                        Subject = "Język francuski",
                        Classes = new[]
                        {
                            new Class("II c")
                        },
                        Groups = "fra",
                        Cancelled = true,
                        Room = "304",
                        Note = string.Empty
                    },
                    new Substitution
                    {
                        Teacher = "Dorota Rutkowska",
                        Lesson = 1,
                        Subject = "Język niemiecki",
                        Classes = new[]
                        {
                            new Class("II b"),
                            new Class("II a")
                        },
                        Groups = "niem2,niem2",
                        Cancelled = true,
                        Room = "206",
                        Note = string.Empty
                    },
                    new Substitution
                    {
                        Teacher = "Magdalena Sikora",
                        Lesson = 1,
                        Subject = "Język francuski",
                        Classes = new[]
                        {
                            new Class("II d")
                        },
                        Groups = "fra",
                        Room = "304",
                        Substituting = "Biblioteka",
                        Note = "biblioteka"
                    },
                    new Substitution
                    {
                        Teacher = "Dorota Rutkowska",
                        Lesson = 2,
                        Subject = "Język niemiecki",
                        Classes = new[]
                        {
                            new Class("Ia2")
                        },
                        Groups = "niem",
                        Room = "208",
                        Substituting = "Emil Cieślak",
                        Note = "z"
                    },
                    new Substitution
                    {
                        Teacher = "Magdalena Sikora",
                        Lesson = 2,
                        Subject = "Język francuski",
                        Classes = new[]
                        {
                            new Class("Ic2")
                        },
                        Groups = "fra",
                        Room = "304",
                        Substituting = "Biblioteka",
                        Note = "biblioteka"
                    },
                    new Substitution
                    {
                        Teacher = "Dorota Rutkowska",
                        Lesson = 3,
                        Subject = "Język niemiecki",
                        Classes = new[]
                        {
                            new Class("Id1")
                        },
                        Groups = "niem",
                        Room = "300",
                        Substituting = "Marcin Dahm",
                        Note = "z"
                    },
                    new Substitution
                    {
                        Teacher = "Magdalena Sikora",
                        Lesson = 5,
                        Subject = "Godzina z wychowawcą",
                        Classes = new[]
                        {
                            new Class("II c")
                        },
                        Groups = "Cała klasa",
                        Room = "304",
                        Substituting = "Beata Pleszewska",
                        Note = "z"
                    },
                    new Substitution
                    {
                        Teacher = "Magdalena Sikora",
                        Lesson = 6,
                        Subject = "Język francuski",
                        Classes = new[]
                        {
                            new Class("III c"),
                            new Class("III d")
                        },
                        Groups = "fra 2,fra 2",
                        Room = "114",
                        Substituting = "Renata Dombrowska",
                        Note = "całość"
                    },
                    new Substitution
                    {
                        Teacher = "Magdalena Sikora",
                        Lesson = 7,
                        Subject = "Język francuski",
                        Classes = new[]
                        {
                            new Class("II a"),
                            new Class("II b")
                        },
                        Groups = "fra,fra",
                        Room = "304",
                        Cancelled = true,
                        Note = string.Empty
                    }
                }
            };
            var parser = new SubstitutionsPlanXmlParser(new XmlParsingModelsMapper());

            var result = await parser.Parse(stream);

            result.Should().BeEquivalentTo(expected,
                options => options.Excluding(x => x.SelectedMemberPath.EndsWith("Id")));
        }

        [Fact]
        public void ThrowsParsingExceptionOnNotValidObject()
        {
            var stream = new MemoryStream(Encoding.Default.GetBytes(
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<note>\r\n  <to>Tove</to>\r\n  <from>Jani</from>\r\n  <heading>Reminder</heading>\r\n  <body>Don't forget me this weekend!</body>\r\n</note>"));
            var parser = new SubstitutionsPlanXmlParser(new XmlParsingModelsMapper());

            Action result = () => parser.Parse(stream).GetAwaiter().GetResult();

            result.Should().Throw<ParsingException>();
        }
    }
}