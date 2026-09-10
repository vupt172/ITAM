// ITAM.Infrastructure/Data/DbSeeder.cs
using ITAM.Domain.Entities.Identity;
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
        }
    }
}