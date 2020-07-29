using System.Xml.Serialization;

namespace eru.Infrastructure.XmlParsing.Models
{
    [XmlRoot("substitutions")]
    public class Substitutions
    {
        [XmlElement("date")] public Date Date { get; set; }
    }
}