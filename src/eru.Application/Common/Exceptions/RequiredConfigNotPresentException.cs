using System;

namespace eru.Application.Common.Exceptions
{
    public class RequiredConfigNotPresentException : Exception
    {
        public RequiredConfigNotPresentException(string configurationKey)
            : base($"Required configuration with key {configurationKey} is not present.")
        {
            
        }
    }
}