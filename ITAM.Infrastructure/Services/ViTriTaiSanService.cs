using ITAM.Domain.Entities;
using ITAM.Domain.Exceptions;
using ITAM.Domain.Interfaces;
using ITAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITAM.Infrastructure.Services
{
    public class ViTriTaiSanService : CatalogService<ViTriTaiSan>, IViTriTaiSanService
    {

        public ViTriTaiSanService(AppDbContext context) : base(context)
        {

        }

        // Override để Include PhongBan khi lấy danh sách
        public override async Task<IEnumerable<ViTriTaiSan>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.Set<ViTriTaiSan>().Include(x => x.PhongBan).AsNoTracking();

            if (!includeInactive)
            {
                query = query.Where(x => x.IsActive);
            }
            return await query.ToListAsync();
        }

        // Override để Include PhongBan khi lấy chi tiết 1 bản ghi
        public override async Task<ViTriTaiSan?> GetByIdAsync(long id)
        {
            return await _context.Set<ViTriTaiSan>()
                .Include(x => x.PhongBan)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public override async Task<ViTriTaiSan> CreateAsync(ViTriTaiSan entity)
        {
            await ValidatePhongBanExistsAndActiveAsync(entity.PhongBanId);
            return await base.CreateAsync(entity);
        }

        public override async Task<bool> UpdateAsync(long id, ViTriTaiSan entity)
        {
            await ValidatePhongBanExistsAndActiveAsync(entity.PhongBanId);
            return await base.UpdateAsync(id, entity);
        }

        // Validate that the PhongBan exists and is active
        private async Task ValidatePhongBanExistsAndActiveAsync(long phongBanId)
        {
            var exists = await _context.Set<PhongBan>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == phongBanId && x.IsActive);

            if (!exists)
            {
                throw new InvalidBusinessRuleException("Phòng ban được chọn không tồn tại hoặc đã ngừng hoạt động!");
            }
        }

        public override Task<bool> DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }


    }
}