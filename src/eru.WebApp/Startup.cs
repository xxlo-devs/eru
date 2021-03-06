using eru.Application;
using eru.Infrastructure;
using eru.PlatformClients.FacebookMessenger;
using eru.WebApp.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace eru.WebApp
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication();
            services.AddInfrastructure(_configuration);

            services.AddFacebookMessenger();
            
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizePage("/admin");
            });
            services
                .AddControllers(options =>
                {
                    options.Filters.Add(new ApiExceptionFilterAttribute());
                })
                .AddRazorRuntimeCompilation()
                .AddXmlSerializerFormatters()
                .UseInfrastructure();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
        {
            app.UsePathBase(configuration["Paths:WebAppPathBase"]);

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(x=>x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseSerilogRequestLogging();

            app.UseRouting();
            
            app.UseStaticFiles();
            
            app.UseInfrastructure(_configuration);
            
            app.UseFacebookMessenger();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapHealthChecks("health");
            });
        }
    }
}