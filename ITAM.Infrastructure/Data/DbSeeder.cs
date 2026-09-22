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
        };

        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync(); // đảm bảo DB đã tạo/migrate

            // 1. Seed Features
            foreach (var (code, name) in DefaultFeatures)
            {
                if (!await context.Features.AnyAsync(f => f.Code == code))
                    context.Features.Add(new Feature { Code = code, Name = name });
            }
            await context.SaveChangesAsync();

            // 2. Seed Role "ADMIN" với đầy đủ quyền
            var adminRole = await context.Roles
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

            // 3. Seed tài khoản Admin mặc định
            if (!await context.Users.AnyAsync(u => u.Username == "admin"))
            {
                var adminUser = new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), // ⚠ đổi ngay sau lần đăng nhập đầu
                    FullName = "Quản trị hệ thống",
                    IsActive = true
                };
                adminUser.UserRoles.Add(new UserRole { Role = adminRole });

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }
            // 4. Seed các Phòng Ban + Vị Trí Tài Sản hệ thống (quan hệ 1-1: mỗi Phòng Ban hệ thống có đúng
            // 1 Vị Trí Tài Sản cùng vai trò). Idempotent theo Code — không tạo trùng nếu đã có sẵn (VD: DB test
            // đã được tạo tay qua UI Danh Mục Phòng Ban trước khi có bước seed này).
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
    }
}