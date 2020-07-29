using System;
using System.Collections.Generic;

namespace eru.Domain.Entity
{
    public class SubstitutionsPlan
    {
        public DateTime Date { get; set; }
        public IEnumerable<Substitution> Substitutions { get; set; }
    }
}