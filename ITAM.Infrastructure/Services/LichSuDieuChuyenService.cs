using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITAM.Infrastructure.Services
{
    /// <summary>
    /// Chỉ đọc — không có logic ghi (ghi lịch sử luôn gắn liền với hành động sinh ra nó: Duyệt Lô Nhập,
    /// Duyệt Điều Chuyển, và sau này Báo Mất/Thanh Lý, nên phần ghi nằm ở đúng service của hành động đó,
    /// không tập trung vào đây).
    /// </summary>
    public class LichSuDieuChuyenService : ILichSuDieuChuyenService
    {
        private readonly AppDbContext _context;

        public LichSuDieuChuyenService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<LichSuDieuChuyenTaiSanDto>> GetByTaiSanIdAsync(long taiSanDinhDanhId, int soLuong = 5)
        {
            return await TruyVanCoBan()
                .Where(x => x.TaiSanDinhDanhId == taiSanDinhDanhId)
                .OrderByDescending(x => x.NgayDuyet)
                .Take(soLuong)
                .Select(ChieuSangDto())
                .ToListAsync();
        }

        public async Task<List<LichSuDieuChuyenTaiSanDto>> TimKiemAsync(LichSuDieuChuyenSearchDto dto)
        {
            var query = TruyVanCoBan();

            if (dto.TaiSanDinhDanhId.HasValue)
                query = query.Where(x => x.TaiSanDinhDanhId == dto.TaiSanDinhDanhId.Value);

            if (dto.ViTriTaiSanId.HasValue)
                query = query.Where(x =>
                    x.ViTriChuyenDiId == dto.ViTriTaiSanId.Value ||
                    x.ViTriChuyenDenId == dto.ViTriTaiSanId.Value);

            if (dto.NguoiDuyetId.HasValue)
                query = query.Where(x => x.NguoiDuyetId == dto.NguoiDuyetId.Value);

            if (dto.TuNgay.HasValue)
                query = query.Where(x => x.NgayDuyet >= dto.TuNgay.Value);

            if (dto.DenNgay.HasValue)
                query = query.Where(x => x.NgayDuyet <= dto.DenNgay.Value);

            return await query
                .OrderByDescending(x => x.NgayDuyet)
                .Select(ChieuSangDto())
                .ToListAsync();
        }

        private IQueryable<Domain.Entities.LichSuDieuChuyenTaiSan> TruyVanCoBan()
        {
            return _context.LichSuDieuChuyenTaiSan
                .Include(x => x.TaiSanDinhDanh)
                .Include(x => x.ViTriChuyenDi)
                .Include(x => x.ViTriChuyenDen)
                .Include(x => x.NguoiDuyet)
                .AsNoTracking();
        }

        private static System.Linq.Expressions.Expression<System.Func<Domain.Entities.LichSuDieuChuyenTaiSan, LichSuDieuChuyenTaiSanDto>> ChieuSangDto()
        {
            return x => new LichSuDieuChuyenTaiSanDto
            {
                Id = x.Id,
                TaiSanDinhDanhId = x.TaiSanDinhDanhId,
                MaTaiSan = x.TaiSanDinhDanh.Code,
                TenTaiSan = x.TaiSanDinhDanh.Name,
                TenViTriChuyenDi = x.ViTriChuyenDi != null ? x.ViTriChuyenDi.Name : null,
                TenViTriChuyenDen = x.ViTriChuyenDen.Name,
                NgayDuyet = x.NgayDuyet,
                TenNguoiDuyet = x.NguoiDuyet.FullName,
                GhiChu = x.GhiChu
            };
        }
    }
}