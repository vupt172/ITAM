using ITAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Infrastructure.Configurations
{
    public class LichSuBienDongVatTuConfiguration : IEntityTypeConfiguration<LichSuBienDongVatTu>
    {
        public void Configure(EntityTypeBuilder<LichSuBienDongVatTu> b)
        {
            b.Property(x => x.Loai).HasConversion<string>().HasMaxLength(20);
            b.HasIndex(x => new { x.HangHoaId, x.ViTriTaiSanId, x.NgayDuyet });
        }
    }
}
