using System;
using System.Collections.Generic;
using System.Linq;
using eru.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Application.Configuration
{
    public static class ConfigExtensions
    {
        public static IServiceCollection LoadConfigs(this IServiceCollection services, IConfiguration configuration)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes()).ToArray();
            var configTypes = types
                .Where(x => typeof(IConfig).IsAssignableFrom(x) && !x.IsInterface)
                .ToArray();
            foreach (var configType in configTypes)
            {
                var configDefaultObject = (IConfig)Activator.CreateInstance(configType);
                var config = configuration.GetSection(configDefaultObject?.ConfigKey).Get(configType);
                if (configDefaultObject == null || config == null && configDefaultObject.Required)
                    throw new RequiredConfigNotPresentException(configDefaultObject?.ConfigKey);
                if(config == null && !configDefaultObject.Required)
                    continue;
                Validate(types, configType, config);
                services.AddSingleton(configType, config);
            }

            return services;
        }
        
        /// <summary>
        /// Finds correct validator and validate the object
        /// </summary>
        /// <param name="types">All types to be searched</param>
        /// <param name="objectType"></param>
        /// <param name="object"></param>
        /// <exception cref="ValidationException"></exception>
        private static void Validate(IEnumerable<Type> types, Type objectType, object @object)
        {
            var validatorType = types.Where(x => x.BaseType == typeof(AbstractValidator<>).MakeGenericType(objectType)).ToArray();
            var validationContextType = typeof(ValidationContext<>).MakeGenericType(objectType);
            var validationContext = (IValidationContext)Activator.CreateInstance(validationContextType, @object);
            var validator = (IValidator) Activator.CreateInstance(validatorType.First());
            var validationResult = validator?.Validate(validationContext);
            
            if(validationResult != null && !validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }
    }
}