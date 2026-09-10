using ITAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITAM.Infrastructure.Configurations
{
    public class LoNhapConfiguration : IEntityTypeConfiguration<LoNhap>
    {
        public void Configure(EntityTypeBuilder<LoNhap> builder)
        {
            builder.ToTable("LoNhap");

            builder.Property(x => x.SoLo).HasMaxLength(50).IsRequired();
            builder.HasIndex(x => x.SoLo).IsUnique();

            builder.Property(x => x.GhiChu).HasMaxLength(1000);
            builder.Property(x => x.TrangThai)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.HasOne(x => x.NhaCungCap)
                .WithMany()
                .HasForeignKey(x => x.NhaCungCapId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.NguoiLapPhieu)
                .WithMany()
                .HasForeignKey(x => x.NguoiLapPhieuId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.ChiTiets)
                .WithOne(x => x.LoNhap)
                .HasForeignKey(x => x.LoNhapId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}