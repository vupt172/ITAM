using ITAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITAM.Infrastructure.Configurations
{
    public class VatTuConfiguration : IEntityTypeConfiguration<VatTu>
    {
        public void Configure(EntityTypeBuilder<VatTu> builder)
        {
            builder.ToTable("VatTu");

            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Code).HasMaxLength(100).IsRequired();
            builder.Property(x => x.DonViTinh).HasMaxLength(50);

            builder.HasIndex(x => x.Code).IsUnique();
            // Một hàng hóa vật tư chỉ có đúng một bản ghi tồn tương ứng.
            builder.HasIndex(x => x.HangHoaId).IsUnique();

            builder.Ignore(x => x.SoLuongKhaDung);

            builder.HasOne(x => x.HangHoa)
                .WithMany()
                .HasForeignKey(x => x.HangHoaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ViTriTaiSan)
                .WithMany()
                .HasForeignKey(x => x.ViTriTaiSanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
