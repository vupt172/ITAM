using ITAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITAM.Infrastructure.Configurations
{
    public class LoNhapChiTietConfiguration : IEntityTypeConfiguration<LoNhapChiTiet>
    {
        public void Configure(EntityTypeBuilder<LoNhapChiTiet> builder)
        {
            builder.ToTable("LoNhapChiTiet");

            builder.Property(x => x.SoLo).HasMaxLength(50).IsRequired();
            builder.HasIndex(x => x.SoLo).IsUnique();

            builder.Property(x => x.DonGia).HasColumnType("decimal(18,2)");
            builder.Property(x => x.TenVatPham).HasMaxLength(500).IsRequired();

            // ThanhTien là property tính toán, không map vào cột DB
            builder.Ignore(x => x.ThanhTien);

            builder.HasOne(x => x.HangHoa)
                .WithMany()
                .HasForeignKey(x => x.HangHoaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.LoaiTaiSan)
                .WithMany()
                .HasForeignKey(x => x.LoaiTaiSanId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}
