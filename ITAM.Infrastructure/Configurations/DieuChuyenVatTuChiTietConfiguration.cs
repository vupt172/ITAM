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
    public class DieuChuyenVatTuChiTietConfiguration : IEntityTypeConfiguration<DieuChuyenVatTuChiTiet>
    {
        public void Configure(EntityTypeBuilder<DieuChuyenVatTuChiTiet> b)
        {
            b.HasOne(x => x.HangHoa).WithMany().HasForeignKey(x => x.HangHoaId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.ViTriChuyenDi).WithMany().HasForeignKey(x => x.ViTriChuyenDiId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.ViTriChuyenDen).WithMany().HasForeignKey(x => x.ViTriChuyenDenId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
