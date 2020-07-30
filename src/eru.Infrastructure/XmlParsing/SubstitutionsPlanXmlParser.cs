using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using eru.Application.Common.Exceptions;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;

namespace eru.Infrastructure.XmlParsing
{
    public class SubstitutionsPlanXmlParser : ISubstitutionsPlanXmlParser
    {
        static SubstitutionsPlanXmlParser()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private static Encoding PolishEncoding => Encoding.GetEncoding(1250);

        public async Task<SubstitutionsPlan> Parse(Stream content)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var xmlReader = XmlReader.Create(content, new XmlReaderSettings {Async = true, IgnoreWhitespace = true});
            var date = DateTime.Today;
            var substitutions = new List<Substitution>();
            var containsCorrectSubstitutionsNode = false;
            var containsCorrectDateNode = false;
            while (!xmlReader.EOF)
            {
                await xmlReader.ReadAsync();
                switch (xmlReader.Name)
                {
                    case "substitutions":
                    {
                        if(xmlReader.HasAttributes)
                            throw new XmlParsingException(typeof(SubstitutionsPlan), "Substitutions node should not have any attributes.");
                        containsCorrectSubstitutionsNode = true;
                        break;
                    }
                    case "date":
                    {
                        if(xmlReader.NodeType == XmlNodeType.EndElement)
                            break;
                        if(xmlReader.AttributeCount != 3)
                            throw new XmlParsingException(typeof(SubstitutionsPlan), "Date node does not have 3 required attributes.");
                        
                        var day = Convert.ToInt32(xmlReader.GetAttribute("day"));
                        var month = Convert.ToInt32(xmlReader.GetAttribute("month"));
                        var year = Convert.ToInt32(xmlReader.GetAttribute("year"));
                        
                        date = new DateTime(year, month, day);
                        containsCorrectDateNode = true;
                        break;
                    }
                    case "subst":
                    {
                        if(xmlReader.AttributeCount != 8 && xmlReader.AttributeCount != 9)
                            throw new XmlParsingException(typeof(SubstitutionsPlan), "Subst node does not have 8 or 9 attributes.");
                        var teacher = xmlReader.GetAttribute("absent");
                        var lesson = Convert.ToInt32(xmlReader.GetAttribute("lesson"));
                        var subject = xmlReader.GetAttribute("subject");
                        var classes = xmlReader.GetAttribute("forms").Split(',');
                        var groups = xmlReader.GetAttribute("groups");
                        var substituting = xmlReader.GetAttribute("substituting");
                        var cancelled = Convert.ToBoolean(Convert.ToInt32(xmlReader.GetAttribute("cancelled")));
                        var note = xmlReader.GetAttribute("note");
                        var room = xmlReader.GetAttribute("room");
                        if(teacher==null || subject == null || classes.Length == 0 || groups == null || note == null || room == null)
                            throw new XmlParsingException(typeof(SubstitutionsPlan), 
                                "Subs node must contain teacher, subject, groups, note and room other than null and non empty valid classes list.");
                        substitutions.Add(new Substitution
                        {
                            Teacher = FixEncoding(teacher, PolishEncoding),
                            Lesson = lesson,
                            Subject = FixEncoding(subject, PolishEncoding),
                            Classes = classes.Select(x=>new Class(x)).ToArray(),
                            Cancelled = cancelled,
                            Groups = FixEncoding(groups, PolishEncoding),
                            Note = FixEncoding(note,PolishEncoding),
                            Room = room,
                            Substituting = substituting != null ? FixEncoding(substituting, PolishEncoding) : null
                        });
                        break;
                    }
                }
            }
            if((date == DateTime.Today && substitutions.Count == 0) || !containsCorrectDateNode || !containsCorrectSubstitutionsNode)
                throw new XmlParsingException(typeof(SubstitutionsPlan), "Your xml file looks like is not parsable to SubstitutionsPlan.");
            return new SubstitutionsPlan
            {
                Date = date,
                Substitutions = substitutions
            };
        }

        private static string FixEncoding(string str, Encoding encoding) =>
            Encoding.Default.GetString(encoding.GetBytes(str));
    }
}