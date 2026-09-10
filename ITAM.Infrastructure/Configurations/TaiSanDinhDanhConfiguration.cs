using ITAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITAM.Infrastructure.Configurations
{
    public class TaiSanDinhDanhConfiguration : IEntityTypeConfiguration<TaiSanDinhDanh>
    {
        public void Configure(EntityTypeBuilder<TaiSanDinhDanh> builder)
        {
            builder.ToTable("TaiSanDinhDanh");

            builder.Property(x => x.Name).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Code).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Serial).HasMaxLength(100);
            builder.Property(x => x.GiaNhap).HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.Code).IsUnique();
            builder.HasIndex(x => x.Serial);

            builder.Property(x => x.TrangThaiTaiSan)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.HasOne(x => x.HangHoa)
                .WithMany()
                .HasForeignKey(x => x.HangHoaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.LoaiTaiSan)
                .WithMany()
                .HasForeignKey(x => x.LoaiTaiSanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.LoNhapChiTiet)
                .WithMany(x => x.TaiSanDinhDanhs)
                .HasForeignKey(x => x.LoNhapChiTietId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ViTriTaiSan)
                .WithMany()
                .HasForeignKey(x => x.ViTriTaiSanId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
