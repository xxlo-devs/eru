using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace eru.Application.Tests
{
    [ExcludeFromCodeCoverage]
    class ChangeToken : IChangeToken
    {
        public ChangeToken()
        {
            HasChanged = false;
            ActiveChangeCallbacks = false;
        }

        public bool HasChanged { get; }

        public bool ActiveChangeCallbacks { get; }

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            throw new NotImplementedException();
        }
    }
    
    [ExcludeFromCodeCoverage]
    class ConfigurationSection : FakeConfiguration,IConfigurationSection
    {
        public ConfigurationSection(string key, string path, string value)
        {
            configuration = new Dictionary<string, string>();
            Key = key;
            Path = path;
            Value = value;
        }

        public string Key { get; }
        public string Path { get; }
        public string Value { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class FakeConfiguration : IConfiguration
    {
        protected Dictionary<string, string> configuration;

        public FakeConfiguration()
        {
            configuration = new Dictionary<string, string>();
            configuration.Add("UploadKey", "D6FFE16E-BF9D-4172-9869-F7EC182172A7");
        }

        public string this[string key]
        {
            get => configuration.Where(x => x.Key == key).FirstOrDefault().Value;
            
            set
            {
                configuration.Add(key, value);
            }
        }

        public IConfigurationSection GetSection(string key)
        {
            return new ConfigurationSection(key, key, configuration.Where(x => x.Key == key).FirstOrDefault().Value);
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new NotImplementedException();
        }

        public IChangeToken GetReloadToken() => new ChangeToken();
        
    }
}
