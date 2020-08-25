using System;

namespace eru.Domain.Entity
{
    public class Class
    {
        private Class()
        {
            //Required by EF
        }

        public Class(int year, string section)
        {
            Year = year;
            Section = section.Trim().ToLower();
        }
        
        public string Id { get; set; }
        public int Year { get; set; }
        public string Section { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Class @class 
                   && @class.Year == Year
                   && @class.Section == Section;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Year.GetHashCode(), Section.GetHashCode());
        }
        
        public override string ToString()
        {
            return string.Format("{0}{1}", Year.ToString(), Section); 
        }
    }
}
