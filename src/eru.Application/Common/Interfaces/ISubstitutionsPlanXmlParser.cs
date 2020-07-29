using System.IO;
using System.Threading.Tasks;
using eru.Domain.Entity;

namespace eru.Application.Common.Interfaces
{
    public interface ISubstitutionsPlanXmlParser
    {
        Task<SubstitutionsPlan> Parse(Stream content);
    }
}