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
    public class PhieuThatLacConfiguration : IEntityTypeConfiguration<PhieuThatLac>
    {
        public void Configure(EntityTypeBuilder<PhieuThatLac> builder)
        {
            builder.ToTable("PhieuThatLac");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SoPhieu).HasMaxLength(30).IsRequired();
            builder.HasIndex(x => x.SoPhieu).IsUnique();   // chặn trùng số phiếu khi 2 người lưu cùng lúc

            builder.Property(x => x.NguoiPhatHien).HasMaxLength(200);
            builder.Property(x => x.LyDo).HasMaxLength(1000).IsRequired();

            builder.Property(x => x.TrangThai)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.HasOne(x => x.PhongBan).WithMany()
                .HasForeignKey(x => x.PhongBanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.NguoiTao).WithMany()
                .HasForeignKey(x => x.NguoiTaoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.NguoiDuyet).WithMany()
                .HasForeignKey(x => x.NguoiDuyetId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.ChiTiets).WithOne(x => x.PhieuThatLac)
                .HasForeignKey(x => x.PhieuThatLacId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
