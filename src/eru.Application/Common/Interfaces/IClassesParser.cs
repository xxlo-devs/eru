using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Domain.Entity;

namespace eru.Application.Common.Interfaces
{
    public interface IClassesParser
    {
        Class Parse(string name);
        IEnumerable<Class> Parse(IEnumerable<string> names);
    }
}