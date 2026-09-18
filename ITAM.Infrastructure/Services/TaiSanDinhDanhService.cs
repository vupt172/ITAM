using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities;
using ITAM.Domain.Enums;
using ITAM.Domain.Exceptions;
using ITAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITAM.Infrastructure.Services
{
    public class TaiSanDinhDanhService : ITaiSanDinhDanhService
    {
        private readonly AppDbContext _context;

        public TaiSanDinhDanhService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TaiSanDinhDanh?> GetByIdAsync(long id)
        {
            return await _context.TaiSanDinhDanh
                .AsNoTracking()
                .Include(x => x.HangHoa)
                .Include(x => x.LoaiTaiSan)
                .Include(x => x.ViTriTaiSan)
                .Include(x => x.LoNhapChiTiet)   // thêm dòng này
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<TaiSanDinhDanh>> GetAllAsync()
        {
            return await _context.TaiSanDinhDanh
                .AsNoTracking()
                .Include(x => x.HangHoa)
                .Include(x => x.LoaiTaiSan)
                .Include(x => x.ViTriTaiSan)
                .Include(x => x.LoNhapChiTiet)   // thêm dòng này
                .OrderBy(x => x.Id)
                .ToListAsync();
        }
        public async Task<List<TaiSanDinhDanh>> GetAllAsync(long phongBanId)
        {
            return await _context.TaiSanDinhDanh
                .AsNoTracking()
                .Include(x => x.HangHoa)
                .ThenInclude(h => h.DMTaiSan)   // ⬅ thêm
                .Include(x => x.LoaiTaiSan)
                .Include(x => x.ViTriTaiSan)
                .Include(x => x.LoNhapChiTiet)
                .Where(x => x.ViTriTaiSan.PhongBanId == phongBanId)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<List<TaiSanDinhDanh>> GetByViTriAsync(long viTriTaiSanId)
        {
            return await _context.TaiSanDinhDanh
                .AsNoTracking()
                .Include(x => x.HangHoa)
                .Include(x => x.LoaiTaiSan)
                .Where(x => x.ViTriTaiSanId == viTriTaiSanId)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }
        public async Task<TaiSanDinhDanh?> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            var ma = code.Trim();

            return await _context.TaiSanDinhDanh
                .AsNoTracking()
                .Include(x => x.HangHoa)
                    .ThenInclude(h => h.DMTaiSan)
                .Include(x => x.LoaiTaiSan)
                .Include(x => x.ViTriTaiSan)
                    .ThenInclude(v => v.PhongBan)   // ⬅ cần thêm để lấy tên khoa phòng khi báo sai lệch
                .Include(x => x.LoNhapChiTiet)
                .FirstOrDefaultAsync(x => x.Code == ma);
        }

        public async Task<List<TaiSanDinhDanh>> GetByTrangThaiAsync(TrangThaiTaiSan trangThai)
        {
            return await _context.TaiSanDinhDanh
                .AsNoTracking()
                .Include(x => x.HangHoa)
                .Include(x => x.LoaiTaiSan)
                .Include(x => x.ViTriTaiSan)
                .Where(x => x.TrangThaiTaiSan == trangThai)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }
        public async Task UpdateAsync(UpdateTaiSanDinhDanhDto dto)
        {
            var taiSan = await _context.TaiSanDinhDanh.FindAsync(dto.Id)
                ?? throw new InvalidBusinessRuleException("Không tìm thấy Tài Sản Định Danh.");

            taiSan.Name = dto.Name;
            taiSan.Serial = dto.Serial;
            taiSan.NamSuDung = dto.NamSuDung;
            taiSan.GhiChu = dto.GhiChu;

            await _context.SaveChangesAsync();
        }


    }
}