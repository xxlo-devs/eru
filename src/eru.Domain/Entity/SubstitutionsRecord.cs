using System;
using System.Collections.Generic;

namespace eru.Domain.Entity
{
    public class SubstitutionsRecord
    {
        public DateTime UploadDateTime { get; set; }
        public DateTime SubstitutionsDate { get; set; }
        public IEnumerable<Substitution> Substitutions { get; set; }
    }
}