using ITAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITAM.Infrastructure.Configurations
{
    public class DieuChuyenChiTietConfiguration : IEntityTypeConfiguration<DieuChuyenChiTiet>
    {
        public void Configure(EntityTypeBuilder<DieuChuyenChiTiet> builder)
        {
            builder.HasOne(x => x.TaiSanDinhDanh)
                .WithMany()
                .HasForeignKey(x => x.TaiSanDinhDanhId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ViTriChuyenDi)
                .WithMany()
                .HasForeignKey(x => x.ViTriChuyenDiId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ViTriChuyenDen)
                .WithMany()
                .HasForeignKey(x => x.ViTriChuyenDenId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}