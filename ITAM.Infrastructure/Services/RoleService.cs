// ITAM.Infrastructure/Services/RoleService.cs
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities.Identity;
using ITAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;

        public RoleService(AppDbContext context) => _context = context;

        public async Task<List<Role>> GetAllAsync()
        {
            return await _context.Roles
                .Include(r => r.RoleFeatures).ThenInclude(rf => rf.Feature)
                .ToListAsync();
        }

        public async Task<Role> CreateAsync(string code, string name, List<long> featureIds)
        {
            if (await _context.Roles.AnyAsync(r => r.Code == code))
                throw new InvalidOperationException($"Mã Role '{code}' đã tồn tại.");

            var role = new Role { Code = code.Trim().ToUpper(), Name = name.Trim() };
            foreach (var fid in featureIds.Distinct())
                role.RoleFeatures.Add(new RoleFeature { FeatureId = fid });

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task UpdateAsync(long roleId, string name, List<long> featureIds)
        {
            var role = await _context.Roles
                .Include(r => r.RoleFeatures)
                .FirstOrDefaultAsync(r => r.Id == roleId)
                ?? throw new InvalidOperationException("Không tìm thấy Role.");

            role.Name = name.Trim();

            var newIds = featureIds.Distinct().ToHashSet();
            var currentIds = role.RoleFeatures.Select(rf => rf.FeatureId).ToHashSet();

            // Xóa các RoleFeature không còn trong danh sách mới
            var toRemove = role.RoleFeatures
                .Where(rf => !newIds.Contains(rf.FeatureId))
                .ToList();                                    // ⬅ ToList() trước để tránh lỗi "Collection was modified"

            foreach (var rf in toRemove)
                role.RoleFeatures.Remove(rf);

            // Thêm các FeatureId mới chưa có
            foreach (var fid in newIds.Except(currentIds))
                role.RoleFeatures.Add(new RoleFeature { RoleId = role.Id, FeatureId = fid });

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
        }
    }
}