using ITAM.Domain.Entities;
using ITAM.Domain.Interfaces;
using ITAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Infrastructure.Services
{
    public class HangHoaService : CatalogService<HangHoa>, IHangHoaService
    {
        public HangHoaService(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<HangHoa>> GetAllWithDanhMucAsync(bool includeInactive = false)
        {
            var query = _context.HangHoa
                .AsNoTracking()
                .Include(x => x.DMTaiSan)
                .AsQueryable();

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            return await query.OrderBy(x => x.Code).ToListAsync();
        }
    }
}
