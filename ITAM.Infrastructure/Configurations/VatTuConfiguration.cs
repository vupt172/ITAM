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

            // Một hàng hóa vật tư chỉ có đúng một bản ghi tồn tương ứng.
            builder.HasIndex(x => new { x.HangHoaId, x.ViTriTaiSanId }).IsUnique();


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
