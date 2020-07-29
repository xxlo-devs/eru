using System;

namespace eru.Domain.Entity
{
    public class Class
    {
        public Class()
        {
            
        }

        public Class(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
        }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}