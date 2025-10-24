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
    internal class WordEntityConfigurations : IEntityTypeConfiguration<WordEntity>
    {
        public void Configure(EntityTypeBuilder<WordEntity> builder)
        {
            builder.HasIndex(w => w.AsString).IsUnique();
            builder.HasQueryFilter(w => !w.IsDeleted);
        }
    }
}
