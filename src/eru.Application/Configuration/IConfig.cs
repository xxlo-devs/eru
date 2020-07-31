namespace eru.Application.Configuration
{
    public interface IConfig
    {
        string ConfigKey { get; }
        bool Required { get; }
    }
}