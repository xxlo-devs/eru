﻿using System.Linq;
using System.Reflection;
using eru.Application.Common.Exceptions;
using eru.Application.Common.Interfaces;
using eru.Infrastructure.Hangfire;
using eru.Infrastructure.Persistence;
using eru.Infrastructure.Translation;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.MemoryStorage;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTranslator();
            
            services.AddDatabase(configuration);
            
            services.AddConfiguredHangfire();
            
            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            
            services.AddTransient<IStopwatch, Stopwatch.Stopwatch>();
            services.AddTransient<IClassesParser, ClassesParser.ClassesParser>();
            services.AddTransient<IHangfireWrapper, HangfireWrapper>();
            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>();
            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IConfiguration configuration) =>
            app
                .UseTranslator()
                .UseConfiguredHangfire(configuration);

        public static IMvcBuilder UseInfrastructure(this IMvcBuilder mvcBuilder)
        {
            return mvcBuilder
                .UseTranslator();
        }
    }
}