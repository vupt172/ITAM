// ITAM.Infrastructure/Data/DbSeeder.cs
using ITAM.Domain.Entities;
using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Entities.Identity;
using ITAM.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Infrastructure.Data
{
    public static class DbSeeder
    {
        // Danh sách Feature mặc định của hệ thống — quản lý tập trung tại đây
        private static readonly (string Code, string Name)[] DefaultFeatures = new[]
        {
            ("DASHBOARD","Xem Dashboard"),
            ("HETHONG_DANHMUC","Hệ thống danh mục"),
            ("HETHONG_NGUOIDUNG","Quản trị người dùng"),
            ("HETHONG_QUYEN","Quản trị phân quyền"),
            ("PHIEUNHAP_NCC","Nhập từ nhà cung cấp"),   // MỚI
            ("DIEUCHUYEN","Điều chuyển tài sản"),        // MỚI
            ("HETHONG_BAOCAO","Hệ thống báo cáo"),
            ("LICHSU_TAISAN","Lịch sử tài sản"),
            ("DANHSACH_TAISAN","Danh sách tài sản"),
            ("DANHSACH_VATU","Danh sách vật tư")
        };
        private static Role? adminRole = null!;

        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync(); // đảm bảo DB đã tạo/migrate
            await SeedFeatureAsync(context);
            await SeedRoleAsync(context);
            await SeedUserAsync(context);
            await SeedPhongBanAndViTriTaiSanAsync(context);
            await SeedLoaiTaiSanAsync(context);

        }
        private static async Task SeedFeatureAsync(AppDbContext context)
        {
            // 1. Seed Features
            foreach (var (code, name) in DefaultFeatures)
            {
                if (!await context.Features.AnyAsync(f => f.Code == code))
                    context.Features.Add(new Feature { Code = code, Name = name });
            }
            await context.SaveChangesAsync();
        }
        private static async Task SeedRoleAsync(AppDbContext context)
        {
            // 2. Seed Role "ADMIN" với đầy đủ quyền
             adminRole = await context.Roles
                .Include(r => r.RoleFeatures)
                .FirstOrDefaultAsync(r => r.Code == "ADMIN");

            var allFeatures = await context.Features.ToListAsync();

            if (adminRole is null)
            {
                adminRole = new Role { Code = "ADMIN", Name = "Quản trị viên" };
                foreach (var f in allFeatures)
                    adminRole.RoleFeatures.Add(new RoleFeature { FeatureId = f.Id });

                context.Roles.Add(adminRole);
                await context.SaveChangesAsync();
            }
            else
            {
                // Đảm bảo Admin luôn có đủ feature mới nhất (nếu sau này thêm feature mới)
                var existingFeatureIds = adminRole.RoleFeatures.Select(rf => rf.FeatureId).ToHashSet();
                foreach (var f in allFeatures.Where(f => !existingFeatureIds.Contains(f.Id)))
                    adminRole.RoleFeatures.Add(new RoleFeature { RoleId = adminRole.Id, FeatureId = f.Id });

                await context.SaveChangesAsync();
            }
        }
        private static async Task SeedUserAsync(AppDbContext context)
        {
            if (!await context.Users.AnyAsync(u => u.Username == "admin"))
            {
                var adminUser = new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), // ⚠ đổi ngay sau lần đăng nhập đầu
                    FullName = "Quản trị hệ thống",
                    IsActive = true
                };
                adminUser.UserRoles.Add(new UserRole { Role = adminRole! });

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }
        }
        private static async Task SeedPhongBanAndViTriTaiSanAsync(AppDbContext context)
        {
            var phongBanHeThong = new (string PhongBanCode, string PhongBanName, string ViTriCode, string ViTriName)[]
{
    (PhongBanCodes.KHO_LUU_TRU,      "Kho Lưu Trữ",      ViTriTaiSanCodes.KHO_LUU_TRU,      "Kho Lưu Trữ"),
    (PhongBanCodes.KHO_THAT_LAC,     "Kho Thất Lạc",     ViTriTaiSanCodes.KHO_THAT_LAC,     "Kho Thất Lạc"),
    (PhongBanCodes.KHO_CHO_THANH_LY, "Kho Chờ Thanh Lý", ViTriTaiSanCodes.KHO_CHO_THANH_LY, "Kho Chờ Thanh Lý"),
    (PhongBanCodes.KHO_THANH_LY,     "Kho Đã Thanh Lý",  ViTriTaiSanCodes.KHO_THANH_LY,     "Kho Đã Thanh Lý"),
};

            foreach (var (phongBanCode, phongBanName, viTriCode, viTriName) in phongBanHeThong)
            {
                var phongBan = await context.PhongBan.FirstOrDefaultAsync(x => x.Code == phongBanCode);
                if (phongBan is null)
                {
                    phongBan = new PhongBan { Code = phongBanCode, Name = phongBanName };
                    context.PhongBan.Add(phongBan);
                    await context.SaveChangesAsync(); // cần Id của PhongBan trước khi gán cho ViTriTaiSan bên dưới
                }

                var viTri = await context.ViTriTaiSan.FirstOrDefaultAsync(x => x.Code == viTriCode);
                if (viTri is null)
                {
                    context.ViTriTaiSan.Add(new ViTriTaiSan
                    {
                        Code = viTriCode,
                        Name = viTriName,
                        PhongBanId = phongBan.Id,
                        IsSystem = true
                    });
                }
                else if (!viTri.IsSystem)
                {
                    // Bản ghi đã có sẵn (tạo tay qua UI trước khi có cột IsSystem) — đảm bảo luôn đúng cờ hệ thống.
                    viTri.IsSystem = true;
                }
            }
            await context.SaveChangesAsync();
        }
        private static async Task SeedLoaiTaiSanAsync(AppDbContext context)
        {
            LoaiTaiSan[] loaiTaiSans =
            {
        new LoaiTaiSan{Code= LoaiTaiSanCodes.TaiSanCoDinh, Name= "Tài Sản Cố Định", MinValue=10000000, MaxValue= 999999999, TyLeHaoMon=20}, // > 10 triệu
        new LoaiTaiSan{Code= LoaiTaiSanCodes.CongCuDungCu, Name= "Công Cụ Dụng Cụ", MinValue=0, MaxValue= 9999999},
        new LoaiTaiSan{Code= LoaiTaiSanCodes.VatTu, Name= "Vật Tư", MinValue=0, MaxValue= 9999999},
            };

            foreach (var loaiTaiSan in loaiTaiSans)
            {
                if (!await context.LoaiTaiSan.AnyAsync(l => l.Code == loaiTaiSan.Code)) context.LoaiTaiSan.AddRange(loaiTaiSans);
            } 
            await context.SaveChangesAsync();
        }
    }
}