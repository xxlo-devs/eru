using System.Xml.Serialization;

namespace eru.Infrastructure.XmlParsing.Models
{
    [XmlRoot(ElementName = "subst")]
    public class Substitution
    {
        [XmlAttribute(AttributeName = "absent")]
        public string Absent { get; set; }

        [XmlAttribute(AttributeName = "lesson")]
        public int Lesson { get; set; }

        [XmlAttribute(AttributeName = "subject")]
        public string Subject { get; set; }

        [XmlAttribute(AttributeName = "forms")]
        public string Forms { get; set; }

        [XmlAttribute(AttributeName = "groups")]
        public string Groups { get; set; }

        [XmlAttribute(AttributeName = "cancelled")]
        public bool Cancelled { get; set; }

        [XmlAttribute(AttributeName = "note")] public string Note { get; set; }

        [XmlAttribute(AttributeName = "room")] public string Room { get; set; }

        [XmlAttribute(AttributeName = "substituting")]
        public string Substituting { get; set; }
    }
}