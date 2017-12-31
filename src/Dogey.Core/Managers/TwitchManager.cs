using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class TwitchManager : DbManager<TwitchDatabase>
    {
        public TwitchManager(TwitchDatabase db)
            : base(db) { }

        public Task<ulong[]> GetStreamIdsAsync()
            => _db.Streams.Select(x => x.Id).ToArrayAsync();
        public Task<TwitchStream[]> GetStreamsAsync()
            => _db.Streams.ToArrayAsync();
        public Task<bool> StreamExistsAsync(ulong streamId)
            => _db.Streams.AnyAsync(x => x.Id == streamId);
        public Task<bool> StreamExistsAsync(string username)
            => _db.Streams.AnyAsync(x => x.Username == username);

        public Task<TwitchStreamChannel[]> GetChannelsAsync(ulong streamId)
            => _db.Channels.Where(x => x.StreamId == streamId).ToArrayAsync();

        public async Task CreateAsync(TwitchStream twitchStream)
        {
            await _db.Streams.AddAsync(twitchStream);
            await _db.SaveChangesAsync();
        }

        public async Task CreateAsync(TwitchStreamChannel twitchStreamChannel)
        {
            await _db.Channels.AddAsync(twitchStreamChannel);
            await _db.SaveChangesAsync();
        }
    }
}
