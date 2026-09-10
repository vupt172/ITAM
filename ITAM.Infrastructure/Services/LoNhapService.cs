using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities;
using ITAM.Domain.Enums;
using ITAM.Domain.Exceptions;
using ITAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Infrastructure.Services
{
    public class LoNhapService : ILoNhapService
    {
        private readonly AppDbContext _context;

        // Code cố định gán lúc seed cho ViTriTaiSan "Kho nhập" — dùng Code thay vì Name
        // để tránh phụ thuộc vào chuỗi hiển thị (có thể đổi tên sau này).
        private const string MA_VI_TRI_KHO_NHAP = "VT_KHO_LUU_TRU";

        public LoNhapService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<long> TaoPhieuNhapAsync(CreateLoNhapDto dto)
        {
            var loNhap = new LoNhap
            {
                SoLo = string.Empty, // gán sau khi có Id
                NgayNhap = dto.NgayNhap,
                NhaCungCapId = dto.NhaCungCapId,
                NguoiLapPhieuId = dto.NguoiLapPhieuId,
                GhiChu = dto.GhiChu,
                TrangThai = TrangThaiLoNhap.PENDING
            };

            _context.LoNhap.Add(loNhap);
            await _context.SaveChangesAsync();

            loNhap.SoLo = $"LN{loNhap.Id:D6}";
            await _context.SaveChangesAsync();

            return loNhap.Id;
        }

        public async Task<long> ThemChiTietAsync(long loNhapId, CreateLoNhapChiTietDto dto)
        {
            var loNhap = await _context.LoNhap
                .Include(x => x.ChiTiets)
                .FirstOrDefaultAsync(x => x.Id == loNhapId)
                ?? throw new InvalidBusinessRuleException("Không tìm thấy Lô Nhập.");

            if (loNhap.TrangThai != TrangThaiLoNhap.PENDING)
                throw new InvalidBusinessRuleException("Chỉ được thêm dòng chi tiết khi phiếu đang ở trạng thái Chờ duyệt.");

            if (dto.SoLuongNhap <= 0)
                throw new InvalidBusinessRuleException("Số lượng nhập phải lớn hơn 0.");

            var hangHoa = await _context.HangHoa
                .Include(x => x.DMTaiSan)
                .FirstOrDefaultAsync(x => x.Id == dto.HangHoaId && x.IsActive)
                ?? throw new InvalidBusinessRuleException("Không tìm thấy Hàng Hóa hoặc hàng hóa đã ngừng sử dụng.");

            var dmTaiSan = hangHoa.DMTaiSan;

            long? loaiTaiSanId = null;

            if (dmTaiSan.IsTrackedById)
            {
                if (dto.LoaiTaiSanId == null)
                    throw new InvalidBusinessRuleException(
                        $"Danh mục '{dmTaiSan.Name}' quản lý theo định danh — bắt buộc chọn Loại Tài Sản.");

                var loaiTaiSan = await _context.LoaiTaiSan.FindAsync(dto.LoaiTaiSanId.Value)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Loại Tài Sản đã chọn.");

                if (dto.DonGia < loaiTaiSan.MinValue || dto.DonGia > loaiTaiSan.MaxValue)
                    throw new InvalidBusinessRuleException(
                        $"Đơn giá {dto.DonGia:N0} không nằm trong ngưỡng của '{loaiTaiSan.Name}' " +
                        $"({loaiTaiSan.MinValue:N0} - {loaiTaiSan.MaxValue:N0}). Chọn lại Loại Tài Sản phù hợp.");

                loaiTaiSanId = loaiTaiSan.Id;
            }

            // Lấy STT lớn nhất đang tồn tại trong các dòng chi tiết còn lại của phiếu (không dùng Count,
            // vì Count giảm sau khi xóa dòng giữa và có thể sinh lại SoLo trùng với dòng đã xóa trước đó).
            var stt = loNhap.ChiTiets
                .Select(ct =>
                {
                    var suffix = ct.SoLo.Contains('-') ? ct.SoLo[(ct.SoLo.LastIndexOf('-') + 1)..] : null;
                    return int.TryParse(suffix, out var n) ? n : 0;
                })
                .DefaultIfEmpty(0)
                .Max() + 1;

            var chiTiet = new LoNhapChiTiet
            {
                LoNhapId = loNhapId,
                SoLo = $"{loNhap.SoLo}-{stt:00}",
                HangHoaId = dto.HangHoaId,
                TenVatPham = hangHoa.Name,
                SoLuongNhap = dto.SoLuongNhap,
                DonGia = dto.DonGia,
                LoaiTaiSanId = loaiTaiSanId
            };

            _context.LoNhapChiTiet.Add(chiTiet);
            await _context.SaveChangesAsync();

            return chiTiet.Id;
        }

        public async Task XoaChiTietAsync(long chiTietId)
        {
            var chiTiet = await _context.LoNhapChiTiet
                .Include(x => x.LoNhap)
                .FirstOrDefaultAsync(x => x.Id == chiTietId)
                ?? throw new InvalidBusinessRuleException("Không tìm thấy dòng chi tiết.");

            if (chiTiet.LoNhap.TrangThai != TrangThaiLoNhap.PENDING)
                throw new InvalidBusinessRuleException("Chỉ được xóa dòng chi tiết khi phiếu đang ở trạng thái Chờ duyệt.");

            _context.LoNhapChiTiet.Remove(chiTiet);
            await _context.SaveChangesAsync();
        }

        public async Task DuyetLoNhapAsync(long loNhapId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var loNhap = await _context.LoNhap
                    .Include(x => x.ChiTiets)
                        .ThenInclude(ct => ct.HangHoa)
                            .ThenInclude(hh => hh.DMTaiSan)
                    .FirstOrDefaultAsync(x => x.Id == loNhapId)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Lô Nhập.");

                if (loNhap.TrangThai != TrangThaiLoNhap.PENDING)
                    throw new InvalidBusinessRuleException("Phiếu đã được duyệt hoặc đã từ chối, không thể duyệt lại.");

                if (!loNhap.ChiTiets.Any())
                    throw new InvalidBusinessRuleException("Lô Nhập chưa có dòng chi tiết nào.");

                var khoNhap = await _context.ViTriTaiSan
                    .FirstOrDefaultAsync(x => x.Code == MA_VI_TRI_KHO_NHAP)
                    ?? throw new InvalidBusinessRuleException(
                        $"Chưa cấu hình Vị Trí Tài Sản có Code = '{MA_VI_TRI_KHO_NHAP}' (Kho nhập) — kiểm tra lại DbSeeder.");

                var taiSanMoiTao = new List<TaiSanDinhDanh>();
                var vatTuMoiTao = new List<VatTu>();

                foreach (var chiTiet in loNhap.ChiTiets)
                {
                    if (chiTiet.HangHoa.DMTaiSan.IsTrackedById)
                    {
                        // LoaiTaiSanId đã được validate & lưu từ lúc ThemChiTietAsync — không suy luận lại ở đây.
                        if (chiTiet.LoaiTaiSanId == null)
                            throw new InvalidBusinessRuleException(
                                $"Dòng '{chiTiet.SoLo}' chưa chọn Loại Tài Sản — không thể duyệt.");

                        for (var i = 0; i < chiTiet.SoLuongNhap; i++)
                        {
                            var taiSan = new TaiSanDinhDanh
                            {
                                Name = chiTiet.HangHoa.Name,
                                // Mã tạm phải duy nhất vì Code có unique index.
                                Code = $"TMP-{Guid.NewGuid():N}",
                                HangHoaId = chiTiet.HangHoaId,
                                Serial = null, // chưa nhập lúc Lô Nhập — điền khi Điều chuyển sang phòng ban
                                NamSuDung = loNhap.NgayNhap.Year,
                                GiaNhap = chiTiet.DonGia,
                                LoaiTaiSanId = chiTiet.LoaiTaiSanId.Value,
                                TrangThaiTaiSan = TrangThaiTaiSan.IN_STOCK,
                                LoNhapChiTietId = chiTiet.Id,
                                ViTriTaiSanId = khoNhap.Id,
                                DangChoMuon = false
                            };

                            _context.TaiSanDinhDanh.Add(taiSan);
                            taiSanMoiTao.Add(taiSan);
                        }
                    }
                    else
                    {
                        var vatTu = await _context.VatTu
                            .FirstOrDefaultAsync(x => x.HangHoaId == chiTiet.HangHoaId);

                        if (vatTu == null)
                        {
                            vatTu = new VatTu
                            {
                                Name = chiTiet.HangHoa.Name,
                                Code = $"TMP-{Guid.NewGuid():N}",
                                HangHoaId = chiTiet.HangHoaId,
                                SoLuongTon = 0,
                                SoLuongChoMuon = 0,
                                ViTriTaiSanId = khoNhap.Id
                            };
                            _context.VatTu.Add(vatTu);
                            vatTuMoiTao.Add(vatTu);
                        }

                        vatTu.SoLuongTon += chiTiet.SoLuongNhap;
                    }

                }

                loNhap.TrangThai = TrangThaiLoNhap.APPROVED;
                await _context.SaveChangesAsync();

                // Id chỉ có sau SaveChangesAsync đầu tiên — gán Code cho các TaiSanDinhDanh vừa tạo.
                foreach (var taiSan in taiSanMoiTao)
                {
                    taiSan.Code = $"TS{taiSan.Id:D8}";
                }

                foreach (var vatTu in vatTuMoiTao)
                {
                    vatTu.Code = $"VT{vatTu.Id:D6}";
                }

                if (taiSanMoiTao.Count > 0 || vatTuMoiTao.Count > 0)
                    await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task TuChoiLoNhapAsync(long loNhapId)
        {
            var loNhap = await _context.LoNhap.FindAsync(loNhapId)
                ?? throw new InvalidBusinessRuleException("Không tìm thấy Lô Nhập.");

            if (loNhap.TrangThai != TrangThaiLoNhap.PENDING)
                throw new InvalidBusinessRuleException(
                    "Chỉ được từ chối phiếu khi đang ở trạng thái Chờ duyệt — phiếu đã duyệt đã sinh tài sản/vật tư, không thể hủy trực tiếp.");

            loNhap.TrangThai = TrangThaiLoNhap.REJECTED;
            await _context.SaveChangesAsync();
        }
        public async Task<List<LoNhap>> TimKiemAsync(LoNhapSearchDto criteria)
        {
            var query = _context.LoNhap
                .AsNoTracking()
                .Include(x => x.NhaCungCap)
                .Include(x => x.NguoiLapPhieu)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.SoLo))
                query = query.Where(x => x.SoLo.Contains(criteria.SoLo));

            if (criteria.TuNgay.HasValue)
                query = query.Where(x => x.NgayNhap.Date >= criteria.TuNgay.Value.Date);

            if (criteria.DenNgay.HasValue)
                query = query.Where(x => x.NgayNhap.Date <= criteria.DenNgay.Value.Date);

            if (criteria.TrangThai.HasValue)
                query = query.Where(x => x.TrangThai == criteria.TrangThai.Value);

            return await query.OrderByDescending(x => x.NgayNhap).ToListAsync();
        }
        public async Task<LoNhap?> GetByIdAsync(long loNhapId)
        {
            return await _context.LoNhap
                .AsNoTracking()
                .Include(x => x.NhaCungCap)
                .Include(x => x.NguoiLapPhieu)
                .Include(x => x.ChiTiets)
                    .ThenInclude(ct => ct.HangHoa)
                .FirstOrDefaultAsync(x => x.Id == loNhapId);
        }

        public async Task<List<LoNhap>> GetAllAsync()
        {
            return await _context.LoNhap
                .AsNoTracking()
                .Include(x => x.NhaCungCap)
                .OrderByDescending(x => x.NgayNhap)
                .ToListAsync();
        }
    }
}
