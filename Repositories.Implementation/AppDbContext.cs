using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Implementation.DbConfigurations;

namespace Repositories.Implementation
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<PlayerEntity> Players { get; set; }
        public DbSet<RoomEntity> Rooms { get; set; }
        public DbSet<WordEntity> Words { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<BaseEntity<string>>().UseTpcMappingStrategy();

            modelBuilder.ApplyConfiguration(new RoomEntityConfigurations());
            modelBuilder.ApplyConfiguration(new PlayerEntityConfigurations());
            modelBuilder.ApplyConfiguration(new WordEntityConfigurations());
            base.OnModelCreating(modelBuilder);
        }
    }
}
