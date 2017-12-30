using System;

namespace Dogey
{
    public class TagOwner
    {
        public ulong Id { get; set; }
        public ulong TagId { get; set; }
        public ulong OwnerId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
