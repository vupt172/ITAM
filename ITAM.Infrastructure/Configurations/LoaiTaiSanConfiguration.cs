using ITAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Infrastructure.Configurations
{
    public class LoaiTaiSanConfiguration : IEntityTypeConfiguration<LoaiTaiSan>
    {
        public void Configure(EntityTypeBuilder<LoaiTaiSan> builder)
        {
            builder.ConfigureCatalog();
            builder.Property(x => x.MinValue).HasPrecision(18, 2);
            builder.Property(x => x.MaxValue).HasPrecision(18, 2);
        }
    }
}
