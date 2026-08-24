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
    public class FeatureConfiguraiton : IEntityTypeConfiguration<Feature>
    {
        public void Configure(EntityTypeBuilder<Feature> builder)
        {
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Code).IsRequired().HasMaxLength(50);
            builder.HasIndex(f => f.Code).IsUnique();
            builder.Property(f => f.Name).IsRequired().HasMaxLength(255);
        }
    }
}
