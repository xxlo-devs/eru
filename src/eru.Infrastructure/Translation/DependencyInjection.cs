using System.Globalization;
using eru.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace eru.Infrastructure.Translation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTranslator(this IServiceCollection services)
        {
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("pl"),
                };
                options.DefaultRequestCulture = new RequestCulture("en");
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