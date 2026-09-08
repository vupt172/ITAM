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

            builder.Property(x => x.SoLuongNhap).HasColumnType("decimal(18,2)");
            builder.Property(x => x.DonGia).HasColumnType("decimal(18,2)");

            // ThanhTien là property tính toán, không map vào cột DB
            builder.Ignore(x => x.ThanhTien);

            builder.HasOne(x => x.DMTaiSan)
                .WithMany()
                .HasForeignKey(x => x.DMTaiSanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}