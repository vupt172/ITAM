using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities;
using ITAM.Domain.Exceptions;
using ITAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITAM.Infrastructure.Services
{
    public class VatTuService : IVatTuService
    {
        private readonly AppDbContext _context;

        public VatTuService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<VatTu?> GetByIdAsync(long id)
        {
            return await _context.VatTu
                .AsNoTracking()
                .Include(x => x.HangHoa)
                .Include(x => x.ViTriTaiSan)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<VatTu>> GetAllAsync()
        {
            return await _context.VatTu
                .AsNoTracking()
                .Include(x => x.HangHoa)
                .Include(x => x.ViTriTaiSan)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<List<VatTu>> GetByViTriAsync(long viTriTaiSanId)
        {
            return await _context.VatTu
                .AsNoTracking()
                .Include(x => x.HangHoa)
                .Where(x => x.ViTriTaiSanId == viTriTaiSanId)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }
        public async Task UpdateAsync(UpdateVatTuDto dto)
        {
            var vatTu = await _context.VatTu.FindAsync(dto.Id)
                ?? throw new InvalidBusinessRuleException("Không tìm thấy Vật Tư.");

            vatTu.Name = dto.Name;
            vatTu.DonViTinh = dto.DonViTinh;
            vatTu.GhiChu = dto.GhiChu;

            await _context.SaveChangesAsync();
        }
    }
}