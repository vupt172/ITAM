using ITAM.Domain.Entities.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Infrastructure.Configurations
{
    public class ViTriTaiSanConfiguration : IEntityTypeConfiguration<ViTriTaiSan>
    {
        public void Configure(EntityTypeBuilder<ViTriTaiSan> builder)
        {
            builder.ConfigureCatalog();

            builder.HasOne(x => x.PhongBan)
                .WithMany(x => x.ViTriTaiSans)
                .HasForeignKey(x => x.PhongBanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
