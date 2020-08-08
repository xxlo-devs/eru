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
    }
}