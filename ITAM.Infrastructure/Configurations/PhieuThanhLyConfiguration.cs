using ITAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITAM.Infrastructure.Data.Configurations
{
    public class PhieuThanhLyConfiguration : IEntityTypeConfiguration<PhieuThanhLy>
    {
        public void Configure(EntityTypeBuilder<PhieuThanhLy> b)
        {
            b.ToTable("PhieuThanhLy");
            b.HasKey(x => x.Id);

            b.Property(x => x.SoPhieu).HasMaxLength(20).IsRequired();
            b.HasIndex(x => x.SoPhieu).IsUnique();
            b.Property(x => x.LyDo).HasMaxLength(500).IsRequired();
            b.Property(x => x.TrangThai).HasConversion<string>().HasMaxLength(20); // TODO xác nhận giống PhieuThatLac

            b.HasOne(x => x.PhongBan).WithMany().HasForeignKey(x => x.PhongBanId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.NguoiTao).WithMany().HasForeignKey(x => x.NguoiTaoId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.NguoiDuyet).WithMany().HasForeignKey(x => x.NguoiDuyetId).OnDelete(DeleteBehavior.Restrict);

            b.HasMany(x => x.ChiTiets).WithOne(c => c.PhieuThanhLy)
                .HasForeignKey(c => c.PhieuThanhLyId).OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class PhieuThanhLyChiTietConfiguration : IEntityTypeConfiguration<PhieuThanhLyChiTiet>
    {
        public void Configure(EntityTypeBuilder<PhieuThanhLyChiTiet> b)
        {
            b.ToTable("PhieuThanhLyChiTiet");
            b.HasKey(x => x.Id);

            b.Property(x => x.GiaThanhLy).HasPrecision(18, 2);
            b.Property(x => x.GhiChu).HasMaxLength(500);
            b.HasIndex(x => new { x.PhieuThanhLyId, x.TaiSanDinhDanhId }).IsUnique();

            b.HasOne(x => x.TaiSanDinhDanh).WithMany().HasForeignKey(x => x.TaiSanDinhDanhId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.ViTriTruoc).WithMany().HasForeignKey(x => x.ViTriTruocId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}