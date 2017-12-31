using Discord.WebSocket;
using NTwitch.Rest;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dogey
{
    public class StreamStatusService
    {
        private readonly DiscordSocketClient _discord;
        private readonly TwitchRestClient _twitch;
        private readonly TwitchManager _manager;

        private Timer _timer;

        public StreamStatusService(
            DiscordSocketClient discord,
            TwitchRestClient twitch,
            TwitchManager manager)
        {
            _discord = discord;
            _twitch = twitch;
            _manager = manager;
        }

        public void Start()
        {
            _timer = new Timer(RunAsync, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30));
        }

        public void Stop()
        {
            _timer.Dispose();
            _timer = null;
        }

        private async void RunAsync(object state)
            => await RunAsync().ConfigureAwait(false);
        private async Task RunAsync()
        {
            var streams = await _manager.GetStreamsAsync();
            if (streams.Count() == 0)
                return;

            var livestreams = await _twitch.GetStreamsAsync(x =>
            {
                x.ChannelIds = streams.Select(y => y.Id).ToArray();
                x.Type = NTwitch.StreamType.Live;
            });

            foreach (var livestream in livestreams)
            {
                var stream = streams.SingleOrDefault(x => x.Id == livestream.Channel.Id);
                if (stream == null)
                    continue;

                var notifChannels = await _manager.GetChannelsAsync(stream.Id);

                foreach (var discordChannel in stream.Channels)
                {
                    var socketChannel = _discord.GetChannel(discordChannel.Id);
                    var textChannel = socketChannel as SocketTextChannel;
                    if (textChannel == null)
                        continue;

                    await textChannel.SendMessageAsync($"**{livestream.Channel.DisplayName}** is now playing **{livestream.Game}** ({livestream.Channel.Url})");
                }
            }
        }
    }
}
