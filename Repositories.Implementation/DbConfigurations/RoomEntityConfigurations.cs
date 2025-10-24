using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementation.DbConfigurations
{
    internal class RoomEntityConfigurations : IEntityTypeConfiguration<RoomEntity>
    {
        public void Configure(EntityTypeBuilder<RoomEntity> builder)
        {
            builder.HasIndex(r => r.Key).IsUnique();
            builder.HasOne(r => r.Creator)
                   .WithOne(c => c.CreatedRoom)
                   .HasForeignKey<PlayerEntity>(p => p.CreatedRoomId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(r => r.Joiner)
                   .WithOne(c => c.JoinedRoom)
                   .HasForeignKey<PlayerEntity>(p => p.JoinedRoomId).IsRequired(false)
                   .OnDelete(DeleteBehavior.ClientSetNull);
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
