using ITAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<LoaiTaiSan> LoaiTaiSan => Set<LoaiTaiSan>();
        public DbSet<DMTaiSan> DMTaiSan => Set<DMTaiSan>();
        public DbSet<NhaCungCap> NhaCungCap => Set<NhaCungCap>();
        public DbSet<PhongBan> PhongBan => Set<PhongBan>();
        public DbSet<ViTriTaiSan> ViTriTaiSan => Set<ViTriTaiSan>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly);
        }
    }
}
