using ITAM.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Infrastructure.Configurations
{
    public class UserPhongBanConfiguration : IEntityTypeConfiguration<UserPhongBan>
    {
        public void Configure(EntityTypeBuilder<UserPhongBan> builder)
        {
            builder.ToTable("UserPhongBans");

            builder.HasKey(upb => new { upb.UserId, upb.PhongBanId });

            builder.HasOne(upb => upb.User)
                  .WithMany(u => u.UserPhongBans)
                  .HasForeignKey(upb => upb.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(upb => upb.PhongBan)
                  .WithMany()   // PhongBan không cần biết những User nào scope tới nó
                  .HasForeignKey(upb => upb.PhongBanId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
