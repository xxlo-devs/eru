using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using eru.Application.Substitutions.Commands;
using Microsoft.AspNetCore.Mvc;

namespace eru.WebApp.Models
{
    public class UploadModel
    {
        [FromBody]
        public SubstitutionsPlanXmlModel XmlModel { get; set; }
        
        [FromHeader(Name = "Api-Key")]
        public string ApiKey { get; set; }
    }
    
    [XmlRoot(ElementName = "subst")]
    public class SubstitutionNode
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

        [XmlAttribute(AttributeName = "note")] 
        public string Note { get; set; }

        [XmlAttribute(AttributeName = "room")] 
        public string Room { get; set; }

        [XmlAttribute(AttributeName = "substituting")]
        public string Substituting { get; set; }

        public SubstitutionDto ToSubstitutionDto()
        {
            return new SubstitutionDto
            {
                Absent = Absent,
                Cancelled = Cancelled,
                ClassesNames = Forms.Split(','),
                Groups = Groups,
                Lesson = Lesson,
                Note = Note,
                Room = Room,
                Subject = Subject,
                Substituting = Substituting
            };
        }
    }

    [XmlRoot(ElementName = "date")]
    public class DateNode
    {
        [XmlElement(ElementName = "subst")] 
        public List<SubstitutionNode> Substitutions { get; set; }

        [XmlAttribute(AttributeName = "day")] 
        public string Day { get; set; }

        [XmlAttribute(AttributeName = "month")]
        public string Month { get; set; }

        [XmlAttribute(AttributeName = "year")] 
        public string Year { get; set; }

        public DateTime GetDateTime()
        {
            return new DateTime(int.Parse(Year), int.Parse(Month), int.Parse(Day));
        }

        public IEnumerable<SubstitutionDto> GetSubstitutionsDto()
        {
            return Substitutions.Select(x => x.ToSubstitutionDto());
        }
    }

    [XmlRoot(ElementName = "substitutions")]
    public class SubstitutionsPlanXmlModel
    {
        [XmlElement(ElementName = "date")] 
        public DateNode DateNode { get; set; }
    }
}