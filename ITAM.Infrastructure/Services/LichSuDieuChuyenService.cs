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

            // Lọc theo Khoa/Phòng Ban Đi/Đến — join qua navigation ViTriChuyenDi/ViTriChuyenDen.PhongBanId,
            // vì LichSuDieuChuyenTaiSan chỉ lưu ViTriTaiSanId, không lưu PhongBanId trực tiếp.
            if (dto.PhongBanChuyenDiId.HasValue)
                query = query.Where(x =>
                    x.ViTriChuyenDi != null &&
                    x.ViTriChuyenDi.PhongBanId == dto.PhongBanChuyenDiId.Value);

            if (dto.PhongBanChuyenDenId.HasValue)
                query = query.Where(x => x.ViTriChuyenDen.PhongBanId == dto.PhongBanChuyenDenId.Value);

            // Vị Trí Đi/Đến match trực tiếp (khác ViTriTaiSanId cũ vốn OR cả 2 chiều) —
            // không cần join qua PhongBan vì ViTriChuyenDiId/ViTriChuyenDenId đã là field trực tiếp.
            if (dto.ViTriChuyenDiId.HasValue)
                query = query.Where(x => x.ViTriChuyenDiId == dto.ViTriChuyenDiId.Value);

            if (dto.ViTriChuyenDenId.HasValue)
                query = query.Where(x => x.ViTriChuyenDenId == dto.ViTriChuyenDenId.Value);

            if (!string.IsNullOrWhiteSpace(dto.TuKhoaTaiSan))
                query = query.Where(x =>
                    x.TaiSanDinhDanh.Code.Contains(dto.TuKhoaTaiSan) ||
                    x.TaiSanDinhDanh.Name.Contains(dto.TuKhoaTaiSan));

            if (dto.NguoiDuyetId.HasValue)
                query = query.Where(x => x.NguoiDuyetId == dto.NguoiDuyetId.Value);

            if (dto.TuNgay.HasValue)
                query = query.Where(x => x.NgayDuyet >= dto.TuNgay.Value);

            if (dto.DenNgay.HasValue)
            {
                var denNgay = dto.DenNgay.Value.Date.AddDays(1);

                query = query.Where(x => x.NgayDuyet < denNgay);
            }

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
                    .ThenInclude(v => v.PhongBan) // TODO xác nhận: navigation ViTriTaiSan.PhongBan tồn tại (chỉ mới xác nhận PhongBanId)
                .Include(x => x.ViTriChuyenDen)
                    .ThenInclude(v => v.PhongBan)
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