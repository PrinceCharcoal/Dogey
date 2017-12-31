using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Dogey
{
    public class TwitchDatabase : DbContext
    {
        public DbSet<TwitchStream> Streams { get; private set; }
        public DbSet<TwitchStreamChannel> Channels { get; private set; }

        public TwitchDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "twitch.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TwitchStream>()
                .HasKey(x => x.Id);
            builder.Entity<TwitchStream>()
                .Property(x => x.Username)
                .IsRequired();
            builder.Entity<TwitchStream>()
                .HasMany(x => x.Channels)
                .WithOne(x => x.Stream);

            builder.Entity<TwitchStreamChannel>()
                .HasKey(x => x.Id);
            builder.Entity<TwitchStreamChannel>()
                .Property(x => x.GuildId)
                .IsRequired();
            builder.Entity<TwitchStreamChannel>()
                .Property(x => x.StreamId)
                .IsRequired();
            builder.Entity<TwitchStreamChannel>()
                .Property(x => x.Timestamp)
                .IsRequired();
            builder.Entity<TwitchStreamChannel>()
                .HasOne(x => x.Stream)
                .WithMany(x => x.Channels);
        }
    }
}
