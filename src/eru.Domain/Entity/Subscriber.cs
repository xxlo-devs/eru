using System;
using System.Collections.Generic;
using System.Text;

namespace eru.Domain.Entity
{
    public class Subscriber
    {
        public string Id { get; set; } 
        public string Platform { get; set; }
        public string PreferredLanguage { get; set; }
        public string Class { get; set; }
    }
}
