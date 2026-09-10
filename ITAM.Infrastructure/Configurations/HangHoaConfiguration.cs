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
            builder.HasIndex(x => x.Code).IsUnique();
            builder.Property(x => x.HangSanXuat).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Model).HasMaxLength(100).IsRequired();
            builder.HasOne(x => x.DMTaiSan).WithMany().HasForeignKey(x => x.DMTaiSanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
