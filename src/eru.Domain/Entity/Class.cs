using eru.Domain.Enums;

namespace eru.Domain.Entity
{
    public class Class
    {
        public Class()
        {
            //Required by EF
        }

        public Class(string name)
        {
            //Required by substitution parser
        }

        public Class(string name, Year year)
        {
            Name = name;
            Year = year;
        }
        
        public string Name { get; set; }
        public Year Year { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Class @class && @class.Name == Name && @class.Year == Year;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}