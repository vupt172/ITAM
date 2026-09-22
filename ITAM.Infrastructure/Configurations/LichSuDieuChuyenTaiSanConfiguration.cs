using ITAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITAM.Infrastructure.Configurations
{
    public class LichSuDieuChuyenTaiSanConfiguration : IEntityTypeConfiguration<LichSuDieuChuyenTaiSan>
    {
        public void Configure(EntityTypeBuilder<LichSuDieuChuyenTaiSan> builder)
        {
            builder.HasOne(x => x.TaiSanDinhDanh)
                .WithMany()
                .HasForeignKey(x => x.TaiSanDinhDanhId)
                .OnDelete(DeleteBehavior.Restrict);

            // Nullable FK — tài sản mới sinh từ Duyệt Lô Nhập chưa từng có vị trí trước đó.
            builder.HasOne(x => x.ViTriChuyenDi)
                .WithMany()
                .HasForeignKey(x => x.ViTriChuyenDiId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(x => x.ViTriChuyenDen)
                .WithMany()
                .HasForeignKey(x => x.ViTriChuyenDenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.NguoiDuyet)
                .WithMany()
                .HasForeignKey(x => x.NguoiDuyetId)
                .OnDelete(DeleteBehavior.Restrict);

            // Nullable FK — null khi bản ghi sinh từ Duyệt Lô Nhập (không gắn với phiếu điều chuyển nào).
            builder.HasOne(x => x.DieuChuyenChiTiet)
                .WithMany()
                .HasForeignKey(x => x.DieuChuyenChiTietId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}