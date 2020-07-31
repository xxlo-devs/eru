namespace eru.Application.Configuration.Database
{
    public class DatabaseConfig : IConfig
    {
        public string ConfigKey => "DatabaseConfig";
        public bool Required => true;
        
        public string Server { get; set; }
        public string Database { get; set; }
        public int Port { get; set; } = 5432;
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Pooling { get; set; }
        public int MinPoolSize { get; set; }
        public int MaxPoolSize { get; set; } = 100;
        public int ConnectionLifetime { get; set; }
    }
}