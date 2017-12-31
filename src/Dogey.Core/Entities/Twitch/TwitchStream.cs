using System.Collections.Generic;

namespace Dogey
{
    public class TwitchStream
    {
        public ulong Id { get; set; }
        public string Username { get; set; }

        // Relationships
        public List<TwitchStreamChannel> Channels { get; set; }
    }
}
