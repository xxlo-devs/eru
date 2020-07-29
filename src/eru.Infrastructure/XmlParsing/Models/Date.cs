using System.Collections.Generic;
using System.Xml.Serialization;

namespace eru.Infrastructure.XmlParsing.Models
{
    [XmlRoot(ElementName = "date")]
    public class Date
    {
        [XmlElement(ElementName = "subst")] public List<Substitution> Substitutions { get; set; }

        [XmlAttribute(AttributeName = "day")] public int Day { get; set; }

        [XmlAttribute(AttributeName = "month")]
        public int Month { get; set; }

        [XmlAttribute(AttributeName = "year")] public int Year { get; set; }
    }
}