using ITAM.Domain.Entities;
using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Entities.Identity;
using ITAM.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;


namespace ITAM.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserContext? _currentUserContext;

        public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserContext? currentUserContext = null) :base(options)
        {
            _currentUserContext = currentUserContext;
        }

        // Catalog
        public DbSet<LoaiTaiSan> LoaiTaiSan => Set<LoaiTaiSan>();
        public DbSet<DMTaiSan> DMTaiSan => Set<DMTaiSan>();
        public DbSet<HangHoa> HangHoa => Set<HangHoa>();
        public DbSet<NhaCungCap> NhaCungCap => Set<NhaCungCap>();
        public DbSet<PhongBan> PhongBan => Set<PhongBan>();
        public DbSet<ViTriTaiSan> ViTriTaiSan => Set<ViTriTaiSan>();
        public DbSet<TaiSanDinhDanh> TaiSanDinhDanh { get; set; }
        public DbSet<VatTu> VatTu { get; set; }
        public DbSet<LoNhap> LoNhap { get; set; }
        public DbSet<LoNhapChiTiet> LoNhapChiTiet { get; set; }
        public DbSet<DieuChuyen> DieuChuyen { get; set; }
        public DbSet<DieuChuyenChiTiet> DieuChuyenChiTiet { get; set; }
        public DbSet<LichSuDieuChuyenTaiSan> LichSuDieuChuyenTaiSan { get; set; }

        // Identity — bổ sung
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Feature> Features => Set<Feature>();
        public DbSet<RoleFeature> RoleFeatures => Set<RoleFeature>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<UserPhongBan> UserPhongBans => Set<UserPhongBan>();


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
        public override int SaveChanges()
        {
            ApplyAudit();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            ApplyAudit();
            return base.SaveChangesAsync(cancellationToken);
        }
        private void ApplyAudit()
        {
            var now = DateTime.UtcNow;
            var userId = _currentUserContext?.Instance?.Id;

            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId;
                }
            }
        }
    }
}
