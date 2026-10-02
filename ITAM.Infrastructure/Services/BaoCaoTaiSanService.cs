using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Infrastructure.Data; // TODO xác nhận: namespace thật của AppDbContext
using Microsoft.EntityFrameworkCore;

namespace ITAM.Infrastructure.Services
{
    public class BaoCaoTaiSanService : IBaoCaoTaiSanService
    {
        private readonly AppDbContext _context;

        public BaoCaoTaiSanService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BaoCaoTaiSanDto>> TimKiemAsync(BaoCaoTaiSanSearchDto dto)
        {
            var query = _context.TaiSanDinhDanh
                .AsNoTracking()
                .Include(x => x.HangHoa)
                .Include(x => x.LoaiTaiSan)
                .Include(x => x.LoNhapChiTiet)
                .Include(x => x.ViTriTaiSan).ThenInclude(v => v.PhongBan)
                .AsQueryable();

            // Lọc theo Ngày duyệt (LoNhap.NgayDuyet), không phải CreatedAt
            if (dto.TuNgay.HasValue)
                query = query.Where(x => x.LoNhapChiTiet.LoNhap.NgayDuyet >= dto.TuNgay.Value); // TODO xác nhận: tên field ngày duyệt trên LoNhap có đúng là "NgayDuyet" không?
            if (dto.DenNgay.HasValue)
                query = query.Where(x => x.LoNhapChiTiet.LoNhap.NgayDuyet < dto.DenNgay.Value.Date.AddDays(1));

            if (dto.PhongBanId.HasValue)
                query = query.Where(x => x.ViTriTaiSan.PhongBanId == dto.PhongBanId.Value);
            if (dto.ViTriTaiSanId.HasValue)
                query = query.Where(x => x.ViTriTaiSanId == dto.ViTriTaiSanId.Value);
            if (dto.LoaiTaiSanId.HasValue)
                query = query.Where(x => x.LoaiTaiSanId == dto.LoaiTaiSanId.Value);
            if (dto.DMTaiSanId.HasValue)
                query = query.Where(x => x.HangHoa.DMTaiSanId == dto.DMTaiSanId.Value); // TODO xác nhận: HangHoa có DMTaiSanId không?
            if (dto.TrangThai.HasValue)
                query = query.Where(x => x.TrangThaiTaiSan == dto.TrangThai.Value);
            if (!string.IsNullOrWhiteSpace(dto.HangSanXuat))
                query = query.Where(x => x.HangHoa.HangSanXuat == dto.HangSanXuat); // TODO xác nhận: field HangSanXuat nằm ở đâu

            var result = await query
                .OrderBy(x => x.ViTriTaiSan.PhongBan.Name)
                .ThenBy(x => x.Code)
                .Select(x => new BaoCaoTaiSanDto
                {
                    MaTaiSan = x.Code,
                    TenTaiSan = x.Name,
                    Serial = x.Serial,
                    MaHangHoa = x.HangHoa.Code,
                    TenLoaiTaiSan = x.LoaiTaiSan.Name,
                    TrangThai = x.TrangThaiTaiSan,
                    TenViTri = x.ViTriTaiSan.Name,
                    TenPhongBan = x.ViTriTaiSan.PhongBan.Name,
                    NamSuDung = x.NamSuDung,
                    SoPhieuNhapChiTiet = x.LoNhapChiTiet.SoLo,
                    GiaNhap = x.GiaNhap,
                    GhiChu = x.GhiChu
                })
                .ToListAsync();

            return result;
        }
    }
}