﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Attributes;
using Dogey.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Module, Name("Owner")]
    [MinPermissions(AccessLevel.Owner)]
    public class OwnerModule
    {
        private DiscordSocketClient _client;

        public OwnerModule(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("lookup")]
        [Description("Make Dogey leave a server.")]
        public async Task Set(IUserMessage msg, string ip)
        {
            var getUrl = new Uri($"http://ip-api.com/");
            string mapsUrl = "https://www.google.com/maps/@{0},{1},15z";
            var message = await msg.Channel.SendMessageAsync("Searching...");

            using (var client = new HttpClient())
            {
                client.BaseAddress = getUrl;
                var response = await client.GetAsync(Uri.EscapeDataString($"json/{ip}"));
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var obj = JObject.Parse(jsonResponse);

                if (obj["status"].ToString() == "success")
                {
                    var lookupmsg = new List<string>();
                    lookupmsg.Add($"**Map**: {string.Format(mapsUrl, obj["lat"].ToString(), obj["lon"].ToString())}");
                    lookupmsg.Add("```erlang");
                    lookupmsg.Add($" Country: {obj["country"].ToString()}");
                    lookupmsg.Add($"  Region: {obj["regionName"].ToString()}");
                    lookupmsg.Add($"    City: {obj["city"].ToString()}");
                    lookupmsg.Add($"     Zip: {obj["zip"].ToString()}");
                    lookupmsg.Add($"Timezone: {obj["timezone"].ToString()}");
                    lookupmsg.Add($"     ISP: {obj["isp"].ToString()}");
                    lookupmsg.Add("```");

                    await message.ModifyAsync((e) =>
                    {
                        e.Content = string.Join("\n", msg);
                    });
                }
                else
                {
                    await message.ModifyAsync((e) =>
                    {
                        e.Content = obj["message"].ToString();
                    });
                }
            }
        }
        
        [Command("debug")]
        public async Task Debug(IUserMessage msg)
        {
            var help = new HelpModule(_client);
            await help.Help(msg, "debug");
        }

        [Module("debug"), Name("Owner")]
        [MinPermissions(AccessLevel.Owner)]
        public class SubCommands
        {
            private DiscordSocketClient _client;

            public SubCommands(DiscordSocketClient client)
            {
                _client = client;
            }

            [Command("say")]
            [Description("Make Dogey say something.")]
            public async Task Say(IUserMessage msg, [Remainder]string text)
            {
                await msg.Channel.SendMessageAsync(text);
            }

            [Command("leave")]
            [Description("Make Dogey leave a server.")]
            public async Task Set(IUserMessage msg, ulong guildId)
            {
                var guild = await _client.GetGuildAsync(guildId);

                await guild.LeaveAsync();
                await msg.Channel.SendMessageAsync($"I left the guild **{guild.Name}** ({guild.Id}).");
            }

            [Command("setusername"), Alias("setuser")]
            [Description("Change Dogey's username.")]
            public async Task Username(IUserMessage msg, [Remainder]string name)
            {
                var self = await _client.GetCurrentUserAsync();

                await self.ModifyAsync(e => e.Username = name);
                await msg.Channel.SendMessageAsync($"I changed my username to `{name}`.");
            }

            [Command("setnickname"), Alias("setnick")]
            [Description("Change Dogey's nickname.")]
            [RequireContext(ContextType.Guild)]
            public async Task Nickname(IUserMessage msg, [Remainder]string name)
            {
                var guild = (msg.Channel as ITextChannel)?.Guild;
                var self = await guild.GetCurrentUserAsync();

                await self.ModifyAsync(e => e.Nickname = name);
                await msg.Channel.SendMessageAsync($"I changed my nickname to `{name}`.");
            }

            [Command("setavatar"), Alias("seticon")]
            [Description("Change Dogey's avatar.")]
            public async Task Avatar(IUserMessage msg, string url)
            {
                var self = await _client.GetCurrentUserAsync();
                var q = Uri.EscapeDataString(url);

                using (var client = new HttpClient())
                {
                    var imagestream = await client.GetStreamAsync(q);
                    await self.ModifyAsync(e => e.Avatar = imagestream);
                }

                await msg.Channel.SendMessageAsync($"My avatar has changed successfully.");
            }

            [Command("setgame"), Alias("setplaying")]
            [Description("Change Dogey's game.")]
            public async Task Game(IUserMessage msg, [Remainder]string game)
            {
                var self = await _client.GetCurrentUserAsync();

                await self.ModifyStatusAsync(x => x.Game = new Game(game));
                await msg.Channel.SendMessageAsync($"I am now playing `{game}`.");
            }
        }
    }
}
