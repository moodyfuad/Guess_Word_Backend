using Guess_Word_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Guess_Word_Backend.Data
{
    public class AppDbContext : DbContext
    {
       

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<GameRoom> GameRooms { get; set; }
        //public DbSet<GameRoomStates> GameRoomStates { get; set; }
        public DbSet<Letter> Letters { get; set; }
        public DbSet<Player> Players{ get; set; }
        public DbSet<Word> Words{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
