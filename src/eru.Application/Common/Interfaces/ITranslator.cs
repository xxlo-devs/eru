using System.Threading.Tasks;
using eru.Domain.Entity;

namespace eru.Application.Common.Interfaces
{
    public interface ITranslator<T>
    {
        Task<string> TranslateString(string key, Language lang);
    }
}