using ITAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITAM.Infrastructure.Configurations
{
    public class HangHoaConfiguration : IEntityTypeConfiguration<HangHoa>
    {
        public void Configure(EntityTypeBuilder<HangHoa> builder)
        {
            builder.ToTable("HangHoa");
            builder.ConfigureCatalog();
            builder.Property(f => f.Code).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Code).IsUnique();
            builder.Property(x => x.HangSanXuat).HasMaxLength(255).IsRequired();
            builder.Property(x => x.Model).HasMaxLength(255).IsRequired();
            builder.Property(x => x.DonViTinh).HasMaxLength(255).IsRequired();
            builder.HasOne(x => x.DMTaiSan).WithMany().HasForeignKey(x => x.DMTaiSanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
