using System;
using System.Collections.Generic;
using MediatR;

namespace eru.Application.Substitutions.Commands
{
    public class UploadSubstitutionsCommand : IRequest
    {
        public DateTime UploadDateTime { get; set; }
        public string Key { get; set; }
        public string IpAddress { get; set; }
        public DateTime SubstitutionsDate { get; set; }
        public IEnumerable<SubstitutionDto> Substitutions { get; set; }

        public override string ToString()
        {
            return "{" + $"Ip Address: {IpAddress}, Key: {Key} " + "}";
        }
    }

    public class SubstitutionDto
    {
        public string Absent { get; set; }
        public int Lesson { get; set; }
        public string Subject { get; set; }
        public IEnumerable<string> ClassesNames { get; set; }
        public string Groups { get; set; }
        public bool Cancelled { get; set; }
        public string Note { get; set; }
        public string Room { get; set; }
        public string Substituting { get; set; }
    }
}