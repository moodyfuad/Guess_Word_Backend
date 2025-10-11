using Guess_Word_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Guess_Word_Backend.Data
{
    public class AppDbContext : DbContext
    {
       

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<GameRoom> GameRooms;
        public DbSet<GameRoomStates> GameRoomStates;
        public DbSet<Letter> Letters;
        public DbSet<Player> Players;
        public DbSet<Word> Words;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
