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
    public class PhieuThatLacChiTietConfiguration : IEntityTypeConfiguration<PhieuThatLacChiTiet>
    {
        public void Configure(EntityTypeBuilder<PhieuThatLacChiTiet> builder)
        {
            builder.ToTable("PhieuThatLacChiTiet");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TrangThaiTruoc)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.GhiChu).HasMaxLength(500);

            // Một tài sản chỉ xuất hiện 1 lần trong 1 phiếu.
            builder.HasIndex(x => new { x.PhieuThatLacId, x.TaiSanDinhDanhId }).IsUnique();

            builder.HasOne(x => x.TaiSanDinhDanh).WithMany()
                .HasForeignKey(x => x.TaiSanDinhDanhId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ViTriTruoc).WithMany()
                .HasForeignKey(x => x.ViTriTruocId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
