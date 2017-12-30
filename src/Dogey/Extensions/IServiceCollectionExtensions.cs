using Microsoft.Extensions.DependencyInjection;
using Octokit;
using System;

namespace Dogey
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection WithTags(this IServiceCollection services)
        {
            services.AddDbContext<TagDatabase>(ServiceLifetime.Transient)
                .AddSingleton<TagService>()
                .AddTransient<TagManager>();
            return services;
        }

        public static IServiceCollection WithPoints(this IServiceCollection services)
        {
            services.AddDbContext<PointsDatabase>(ServiceLifetime.Transient)
                .AddSingleton<PointsService>()
                .AddTransient<PointsManager>();
            return services;
        }

        public static IServiceCollection WithScripts(this IServiceCollection services)
        {
            services.AddDbContext<ScriptDatabase>(ServiceLifetime.Transient)
                .AddSingleton<RoslynService>()
                .AddTransient<ScriptManager>();
            return services;
        }

        public static IServiceCollection WithDogs(this IServiceCollection services)
        {
            services.AddDbContext<DogDatabase>(ServiceLifetime.Transient)
                .AddDbContext<PatsDatabase>(ServiceLifetime.Transient)
                .AddSingleton<ChannelWatcher>()
                .AddTransient<DogManager>();
            return services;
        }
    }
}
