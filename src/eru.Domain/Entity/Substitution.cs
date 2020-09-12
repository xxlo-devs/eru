using System.Collections.Generic;

namespace eru.Domain.Entity
{
    public class Substitution
    {
        public string Id { get; set; }
        public string Teacher { get; set; }
        public int Lesson { get; set; }
        public string Subject { get; set; }
        public IEnumerable<Class> Classes { get; set; }
        public string Groups { get; set; }
        public bool Cancelled { get; set; }
        public string Substituting { get; set; }
        public string Note { get; set; }
        public string Room { get; set; }
    }
}