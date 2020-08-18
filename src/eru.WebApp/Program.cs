using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace eru.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(GetConfiguration())
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        
        /// <summary>
        /// Makes configuration for serilog (Environment Variable [For docker] => Json file [For normal hosting])
        /// </summary>
        /// <returns></returns>
        private static IConfiguration GetConfiguration()
        {
            var builder = new HostBuilder();
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables("DOTNET_")
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return config.Build();
        }
    }
}