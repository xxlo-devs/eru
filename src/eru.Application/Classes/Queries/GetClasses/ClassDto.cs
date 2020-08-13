using eru.Application.Common.Mappings;
using eru.Domain.Entity;
using eru.Domain.Enums;

namespace eru.Application.Classes.Queries.GetClasses
{
    public class ClassDto : IMapFrom<Class>
    {
        public string Name { get; set; }
        public Year Year { get; set; }
    }
}