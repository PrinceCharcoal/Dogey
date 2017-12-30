using System;

namespace Dogey
{
    public class TagEdit
    {
        public ulong Id { get; set; }
        public ulong TagId { get; set; }
        public ulong EditorId { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Content { get; set; }

        // Foreign Keys
        public Tag Tag { get; set; }
    }
}
