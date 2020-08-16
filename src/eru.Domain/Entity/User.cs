using System;
using System.Collections.Generic;
using System.Text;
using eru.Domain.Enums;

namespace eru.Domain.Entity
{
    public class User
    {
        public string Id { get; set; } 
        public string Platform { get; set; }

        public string Class { get; set; }
        public Stage Stage { get; set; }
    }
}
