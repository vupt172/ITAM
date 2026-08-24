using BCrypt.Net;
using ITAM.Domain.Entities.Identity;
using ITAM.Infrastructure.Data;
using ITAM.WPF.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace ITAM.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RoleFeatures)
                            .ThenInclude(rf => rf.Feature)
                .Include(u => u.PhongBan)
                .Include(u => u.UserPhongBans)
                .ThenInclude(upb => upb.PhongBan)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user is null)
                throw new AuthenticationException("Tài khoản không tồn tại.");

            if (!user.IsActive)
                throw new AuthenticationException("Tài khoản đã bị khóa.");

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new AuthenticationException("Mật khẩu không đúng.");

            user.LastLoginAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return user;
        }


    }
}