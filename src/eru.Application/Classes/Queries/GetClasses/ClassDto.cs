using AutoMapper;
using eru.Application.Common.Mappings;
using eru.Domain.Entity;

namespace eru.Application.Classes.Queries.GetClasses
{
    public class ClassDto : IMapFrom<Class>
    {
        public string Name { get; set; }
    }
}