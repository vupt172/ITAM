// ITAM.Infrastructure/Services/UserService.cs
using ITAM.AppCore.DTOs.Identity;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities.Identity;
using ITAM.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace ITAM.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.PhongBan)
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.UserPhongBans).ThenInclude(upb => upb.PhongBan)   // ⬅ thêm
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(long id)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.PhongBan)
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.UserPhongBans).ThenInclude(upb => upb.PhongBan)   // ⬅ thêm
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<long>> GetAccessiblePhongBanIdsAsync(long userId)
        {
            return await _context.Set<UserPhongBan>()
                .Where(upb => upb.UserId == userId)
                .Select(upb => upb.PhongBanId)
                .ToListAsync();
        }

        public async Task SetPhongBanAccessAsync(long userId, long? defaultPhongBanId, List<long> accessiblePhongBanIds)
        {
            var distinctIds = accessiblePhongBanIds.Distinct().ToList();

            if (defaultPhongBanId.HasValue && !distinctIds.Contains(defaultPhongBanId.Value))
                throw new InvalidOperationException("Phòng ban mặc định phải nằm trong danh sách Phòng ban được truy cập.");

            var user = await _context.Users
                .Include(u => u.UserPhongBans)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new InvalidOperationException("Không tìm thấy người dùng.");

            user.PhongBanId = defaultPhongBanId;

            var newIds = distinctIds.ToHashSet();
            var currentIds = user.UserPhongBans.Select(upb => upb.PhongBanId).ToHashSet();

            var toRemove = user.UserPhongBans.Where(upb => !newIds.Contains(upb.PhongBanId)).ToList();
            foreach (var upb in toRemove)
                user.UserPhongBans.Remove(upb);

            foreach (var pbId in newIds.Except(currentIds))
                user.UserPhongBans.Add(new UserPhongBan { UserId = user.Id, PhongBanId = pbId });

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUsernameExistsAsync(string username, long? excludeUserId = null)
        {
            return await _context.Users.AnyAsync(u =>
                u.Username == username && (excludeUserId == null || u.Id != excludeUserId));
        }

        public async Task<User> CreateAsync(CreateUserDto dto)
        {
            if (await IsUsernameExistsAsync(dto.Username))
                throw new InvalidOperationException($"Tên đăng nhập '{dto.Username}' đã tồn tại.");

            var user = new User
            {
                Username = dto.Username.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName.Trim(),
                PhongBanId = dto.PhongBanId,
                IsActive = true
            };

            foreach (var roleId in dto.RoleIds.Distinct())
                user.UserRoles.Add(new UserRole { RoleId = roleId });

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(UpdateUserDto dto)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == dto.Id)
                ?? throw new InvalidOperationException("Không tìm thấy người dùng.");

            user.FullName = dto.FullName.Trim();
            user.IsActive = dto.IsActive;
            user.PhongBanId = dto.PhongBanId;

            // Đồng bộ lại danh sách Role
            var newRoleIds = dto.RoleIds.Distinct().ToHashSet();
            var currentRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToHashSet();

            // Xóa role bị bỏ chọn
            var toRemove = user.UserRoles.Where(ur => !newRoleIds.Contains(ur.RoleId)).ToList();
            foreach (var ur in toRemove)
                user.UserRoles.Remove(ur);

            // Thêm role mới
            foreach (var roleId in newRoleIds.Except(currentRoleIds))
                user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleId });

            await _context.SaveChangesAsync();
        }

        public async Task SetActiveAsync(long userId, bool isActive)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new InvalidOperationException("Không tìm thấy người dùng.");
            user.IsActive = isActive;
            await _context.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(ChangePasswordDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId)
                ?? throw new InvalidOperationException("Không tìm thấy người dùng.");

            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
                throw new InvalidOperationException("Mật khẩu cũ không đúng.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
        }

        public async Task ResetPasswordAsync(long userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new InvalidOperationException("Không tìm thấy người dùng.");
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();
        }
        public async Task SetDefaultPhongBanAsync(long userId, long phongBanId)
        {
            var accessibleIds = await GetAccessiblePhongBanIdsAsync(userId);
            if (!accessibleIds.Contains(phongBanId))
                throw new InvalidOperationException("Bạn không có quyền truy cập Phòng ban này.");

            var user = await _context.Users.FindAsync(userId)
                ?? throw new InvalidOperationException("Không tìm thấy người dùng.");

            user.PhongBanId = phongBanId;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(long userId)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new InvalidOperationException("Không tìm thấy người dùng.");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}