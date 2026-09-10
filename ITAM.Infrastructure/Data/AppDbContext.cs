using ITAM.Domain.Entities;
using ITAM.Domain.Entities.Identity;
using ITAM.Infrastructure.Configurations;
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

        // Catalog
        public DbSet<LoaiTaiSan> LoaiTaiSan => Set<LoaiTaiSan>();
        public DbSet<DMTaiSan> DMTaiSan => Set<DMTaiSan>();
        public DbSet<HangHoa> HangHoa => Set<HangHoa>();
        public DbSet<NhaCungCap> NhaCungCap => Set<NhaCungCap>();
        public DbSet<PhongBan> PhongBan => Set<PhongBan>();
        public DbSet<ViTriTaiSan> ViTriTaiSan => Set<ViTriTaiSan>();

        // Identity — bổ sung
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Feature> Features => Set<Feature>();
        public DbSet<RoleFeature> RoleFeatures => Set<RoleFeature>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<UserPhongBan> UserPhongBans => Set<UserPhongBan>();
        public DbSet<TaiSanDinhDanh> TaiSanDinhDanh { get; set; }
        public DbSet<VatTu> VatTu { get; set; }
        public DbSet<LoNhap> LoNhap { get; set; }
        public DbSet<LoNhapChiTiet> LoNhapChiTiet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Các Entity chỉ cần cấu hình chung
            modelBuilder.Entity<DMTaiSan>().ConfigureCatalog();

            modelBuilder.Entity<NhaCungCap>().ConfigureCatalog();

            modelBuilder.Entity<PhongBan>().ConfigureCatalog();
            // Tự động áp dụng các Configuration có sẵn
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
