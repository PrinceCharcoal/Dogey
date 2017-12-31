using System;

namespace Dogey
{
    public class TwitchStreamChannel
    {
        public ulong Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong StreamId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Relationships
        public TwitchStream Stream { get; set; }
    }
}
