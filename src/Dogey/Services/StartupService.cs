using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using NTwitch.Rest;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey
{
    public class StartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly TwitchRestClient _twitch;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        
        public StartupService(
            DiscordSocketClient discord,
            TwitchRestClient twitch,
            CommandService commands,
            IConfigurationRoot config)
        {
            _config = config;
            _discord = discord;
            _twitch = twitch;
            _commands = commands;
        }
        
        public async Task StartAsync()
        {
            await _discord.LoginAsync(TokenType.Bot, _config["tokens:discord"]);
            await _discord.StartAsync();

            await _twitch.LoginAsync(_config["tokens:twitch"]);

            _commands.AddTypeReader<Uri>(new UriTypeReader());
            _commands.AddTypeReader<Tag>(new TagTypeReader());

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

    }
}
