using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using Microsoft.Extensions.Configuration;

namespace eru.Infrastructure.Translation
{
    public class ApplicationCultures : IApplicationCultures
    {
        private readonly IConfiguration _configuration;
        
        public ApplicationCultures(IConfiguration configuration)
        {
            _configuration = configuration;
        } 
        
        public IEnumerable<Language> AvailableCultures { get; } = new[]
        {
            new Language("English", "https://www.countryflags.io/us/flat/64.png", new CultureInfo("en")),
            new Language("Polski", "https://www.countryflags.io/pl/flat/64.png", new CultureInfo("pl"))
        };

        public Language DefaultCulture
            => AvailableCultures.First(x => x.Culture.Name == _configuration["DefaultCulture"]);

        public Language FindCulture(string name)
            => AvailableCultures.FirstOrDefault(x => x.Culture.Name == name);
    }
}