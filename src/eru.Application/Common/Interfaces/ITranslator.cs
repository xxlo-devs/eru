using System.Threading.Tasks;

namespace eru.Application.Common.Interfaces
{
    public interface ITranslator<T>
    {
        Task<string> TranslateString(string key, string culture);
    }
}