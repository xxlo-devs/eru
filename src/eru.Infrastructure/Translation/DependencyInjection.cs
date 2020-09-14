using System.Globalization;
using System.Linq;
using eru.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace eru.Infrastructure.Translation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationCultures(this IServiceCollection services)
        {
            services.AddTransient<IApplicationCultures, ApplicationCultures>();

            return services;
        }
        
        public static IServiceCollection AddTranslator(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var applicationCultures = new ApplicationCultures(configuration);
                var cultures = applicationCultures.AvailableCultures
                    .Select(x => x.Culture)
                    .ToList();
                
                options.DefaultRequestCulture = new RequestCulture(applicationCultures.DefaultCulture.Culture);
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });
            
            services.AddTransient(typeof(ITranslator<>), typeof(Translator<>));
            
            return services;
        }

        public static IApplicationBuilder UseTranslator(this IApplicationBuilder app)
        {
            app.UseRequestLocalization(app.ApplicationServices
                .GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
            
            return app;
        }

        public static IMvcBuilder UseTranslator(this IMvcBuilder mvcBuilder)
        {
            return mvcBuilder
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
        }
    }
}