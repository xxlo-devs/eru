using eru.Application.Common.Mappings;
using eru.Domain.Entity;

namespace eru.Application.Classes.Queries.GetClasses
{
    public class ClassDto : IMapFrom<Class>
    {
        public string Id { get; set; }
        public int Year { get; set; }
        public string Section { get; set; }

        public override string ToString()
        {
            return $"{Year}{Section}";
        }
    }
}