using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Domain.Entity;

namespace eru.Application.Common.Interfaces
{
    public interface IClassesParser
    {
        Task<Class> Parse(string name);
        Task<IEnumerable<Class>> Parse(IEnumerable<string> names);
    }
}