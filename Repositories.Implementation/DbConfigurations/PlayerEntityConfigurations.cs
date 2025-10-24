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
    internal class PlayerEntityConfigurations : IEntityTypeConfiguration<PlayerEntity>
    {
        public void Configure(EntityTypeBuilder<PlayerEntity> builder)
        {
            //builder.HasKey(p => p.Id);
            //builder.HasIndex(p => p.UserId)
            //       .IsUnique();
            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasOne(p => p.CreatedRoom)
                   .WithOne(r => r.Creator)
                   .HasForeignKey<RoomEntity>(r => r.CreatorId).IsRequired(false)
                   .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(p => p.JoinedRoom)
                   .WithOne(r => r.Joiner)
                   .HasForeignKey<RoomEntity>(r => r.JoinerId).IsRequired(false)
                   .OnDelete(DeleteBehavior.ClientSetNull);
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
