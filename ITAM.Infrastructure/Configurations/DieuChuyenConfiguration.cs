using ITAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITAM.Infrastructure.Configurations
{
    public class DieuChuyenConfiguration : IEntityTypeConfiguration<DieuChuyen>
    {
        public void Configure(EntityTypeBuilder<DieuChuyen> builder)
        {
            builder.Property(x => x.SoPhieu).IsRequired().HasMaxLength(30);
            builder.HasIndex(x => x.SoPhieu).IsUnique();

            builder.Property(x => x.TrangThai)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(x => x.NguoiGiao).HasMaxLength(100);
            builder.Property(x => x.NguoiNhan).HasMaxLength(100);

            builder.HasOne(x => x.PhongBanChuyenDi)
                .WithMany()
                .HasForeignKey(x => x.PhongBanChuyenDiId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PhongBanChuyenDen)
                .WithMany()
                .HasForeignKey(x => x.PhongBanChuyenDenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.NguoiTao)
                .WithMany()
                .HasForeignKey(x => x.NguoiTaoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.NguoiDuyet)
                .WithMany()
                .HasForeignKey(x => x.NguoiDuyetId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.ChiTiets)
                .WithOne(x => x.DieuChuyen)
                .HasForeignKey(x => x.DieuChuyenId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}