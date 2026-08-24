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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(u => u.Username)
                .IsUnique(); // Data Annotation không làm được cái này!

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(60);

            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);
            builder.HasOne(u => u.PhongBan)
                .WithMany()
                .HasForeignKey(u => u.PhongBanId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
