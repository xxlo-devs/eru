using System.Globalization;

namespace eru.Domain.Entity
{
    public class Language
    {
        public Language(string name, string flagUrl, CultureInfo culture)
        {
            Name = name;
            FlagUrl = flagUrl;
            Culture = culture;
        }
        
        public string Name { get; set; }
        public string FlagUrl { get; set; }
        public CultureInfo Culture { get; set; }
    }
}