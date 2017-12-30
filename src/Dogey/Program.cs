using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using System;
using System.Threading.Tasks;

namespace Dogey
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        private IConfigurationRoot _config;

        public async Task StartAsync()
        {
            PrettyConsole.NewLine($"Dogey v{AppHelper.Version}");
            PrettyConsole.NewLine();

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("_configuration.json");
            _config = builder.Build();

            var services = new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 100
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    DefaultRunMode = RunMode.Async,
                    LogLevel = LogSeverity.Verbose
                }))
                .AddSingleton(new GitHubClient(new ProductHeaderValue("Dogey"))
                {
                    Credentials = new Credentials(_config["tokens:github"])
                })
                .AddSingleton(new CustomsearchService(new BaseClientService.Initializer()
                {
                    ApiKey = _config["tokens:google"],
                    MaxUrlLength = 256
                }))
                .AddSingleton(new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = _config["tokens:google"],
                    MaxUrlLength = 256
                }))
                .AddDbContext<ConfigDatabase>(ServiceLifetime.Transient)
                .AddTransient<ConfigManager>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<LoggingService>()
                .AddSingleton<StartupService>()
                .AddSingleton<Random>()
                .AddSingleton(_config)
                .WithDogs()
                .WithPoints()
                .WithScripts()
                .WithTags();

            var provider = services.BuildServiceProvider();
            provider.GetRequiredService<LoggingService>();

            await provider.GetRequiredService<StartupService>().StartAsync();
            
            provider.GetRequiredService<CommandHandler>();
            provider.GetRequiredService<PointsService>();
            provider.GetRequiredService<ChannelWatcher>();
            provider.GetRequiredService<TagService>();

            await Task.Delay(-1);
        }
    }
}