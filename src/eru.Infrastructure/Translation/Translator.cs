using System.Globalization;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using Microsoft.Extensions.Localization;

namespace eru.Infrastructure.Translation
{
    public class Translator<T> : ITranslator<T> where T : class
    {
        private readonly IStringLocalizer<T> _localizer;

        public Translator(IStringLocalizer<T> localizer)
        {
            _localizer = localizer;
        }

        public Task<string> TranslateString(string key, string culture)
        {
#pragma warning disable 618
            var localizedString = _localizer.WithCulture(CultureInfo.GetCultureInfo(culture)).GetString(key);
#pragma warning restore 618
            return Task.FromResult(localizedString.ResourceNotFound ? null : localizedString.Value);
        }
    }
}