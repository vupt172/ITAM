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
    public class DieuChuyenVatTuConfiguration : IEntityTypeConfiguration<DieuChuyenVatTu>
    {
        public void Configure(EntityTypeBuilder<DieuChuyenVatTu> b)
        {
            b.Property(x => x.SoPhieu).HasMaxLength(30).IsRequired();
            b.HasIndex(x => x.SoPhieu).IsUnique();
            b.Property(x => x.TrangThai).HasConversion<string>().HasMaxLength(20);
            b.HasOne(x => x.PhongBanChuyenDi).WithMany().HasForeignKey(x => x.PhongBanChuyenDiId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.PhongBanChuyenDen).WithMany().HasForeignKey(x => x.PhongBanChuyenDenId).OnDelete(DeleteBehavior.Restrict);
            b.HasMany(x => x.ChiTiets).WithOne(x => x.DieuChuyenVatTu).HasForeignKey(x => x.DieuChuyenVatTuId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
