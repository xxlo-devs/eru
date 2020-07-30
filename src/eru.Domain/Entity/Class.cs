using System.ComponentModel.DataAnnotations;

namespace eru.Domain.Entity
{
    public class Class
    {
        public Class()
        {
            
        }

        public Class(string name)
        {
            Name = name;
        }
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}