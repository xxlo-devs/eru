using System;
using System.Linq;
using eru.Domain.Entity;
using eru.Infrastructure.XmlParsing.Models;
using Substitution = eru.Domain.Entity.Substitution;
using SubstitutionsPlan = eru.Domain.Entity.SubstitutionsPlan;

namespace eru.Infrastructure.XmlParsing
{
    public class XmlParsingModelsMapper
    {
        public SubstitutionsPlan MapToSubstitutionsPlan(Date model)
        {
            return new SubstitutionsPlan
            {
                Date = new DateTime(model.Year, model.Month, model.Day),
                Substitutions = model.Substitutions.Select(MapToSubstitution)
            };
        }

        public Substitution MapToSubstitution(Models.Substitution model)
        {
            return new Substitution
            {
                Cancelled = model.Cancelled,
                Classes = model.Forms.Split(',').Select(x => new Class(x)),
                Groups = model.Groups,
                Lesson = model.Lesson,
                Note = model.Note,
                Room = model.Room,
                Subject = model.Subject,
                Substituting = model.Substituting,
                Teacher = model.Absent
            };
        }
    }
}