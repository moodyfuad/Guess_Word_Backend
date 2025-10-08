using Guess_Word_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WordleServer.Models;

namespace WordleServer.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Guess> Guesses { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .HasIndex(g => g.GameKey)
                .IsUnique();

            modelBuilder.Entity<Player>()
                .HasIndex(p => new { p.GameId, p.ClientId })
                .IsUnique();

            // Concurrency token to avoid race conditions
            modelBuilder.Entity<Game>()
                .Property(g => g.RowVersion)
                .IsRowVersion();

            base.OnModelCreating(modelBuilder);
        }
    }
}
