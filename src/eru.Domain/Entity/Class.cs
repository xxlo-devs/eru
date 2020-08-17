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
            Name = name;
        }
        
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Class @class && @class.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}