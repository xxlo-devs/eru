using System.Collections.Generic;
using eru.Domain.Entity;

namespace eru.Application.Common.Interfaces
{
    public interface IApplicationCultures
    {
        public IEnumerable<Language> AvailableCultures { get; }
        public Language DefaultCulture { get; }
        public Language FindCulture(string name);
    }
}