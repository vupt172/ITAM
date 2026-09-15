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
        // Code cố định của bản ghi LoaiTaiSan đại diện Vật Tư trong DbSeeder.
        private const string MA_LOAI_VAT_TU = "VATTU";

        private readonly AppDbContext _context;

        // Code cố định gán lúc seed cho ViTriTaiSan "Kho nhập" — dùng Code thay vì Name
        // để tránh phụ thuộc vào chuỗi hiển thị (có thể đổi tên sau này).
        private const string MA_VI_TRI_KHO_NHAP = "VT_KHO_LUU_TRU";

        public LoNhapService(AppDbContext context)
        {
            _context = context;
        }

        // ===================== CÁC METHOD CŨ — giữ nguyên, không còn được UI gọi trực tiếp =====================

        // ===================== METHOD MỚI — UI (LoNhapViewModel) đang dùng =====================

        /// <summary>
        /// Lưu toàn bộ Phiếu Nhập (header + chi tiết) trong 1 transaction — tạo mới nếu Id=null,
        /// cập nhật nếu có Id (chỉ khi phiếu còn PENDING). Đồng bộ chi tiết: thêm dòng mới (Id=null),
        /// cập nhật dòng đã có (Id khác null), xóa các dòng trong ChiTietIdsXoa.
        /// TenVatPham lấy từ dữ liệu người dùng tự nhập trên UI, không tự suy ra từ HangHoa.Name.
        /// Validate lại toàn bộ ở server (không tin dữ liệu từ client). Trả về Id phiếu.
        /// </summary>
        public async Task<long> LuuPhieuNhapAsync(SaveLoNhapDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                LoNhap loNhap;

                if (dto.Id == null)
                {
                    loNhap = new LoNhap
                    {
                        SoLo = string.Empty,
                        NgayNhap = dto.NgayNhap,
                        NhaCungCapId = dto.NhaCungCapId,
                        NguoiLapPhieuId = dto.NguoiLapPhieuId,
                        GhiChu = dto.GhiChu,
                        TrangThai = TrangThaiLoNhap.PENDING
                    };
                    _context.LoNhap.Add(loNhap);
                    await _context.SaveChangesAsync();

                    loNhap.SoLo = await TinhSoPhieuMoiAsync(loNhap.Id, dto.NgayNhap);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    loNhap = await _context.LoNhap
                        .Include(x => x.ChiTiets)
                        .FirstOrDefaultAsync(x => x.Id == dto.Id.Value)
                        ?? throw new InvalidBusinessRuleException("Không tìm thấy Lô Nhập.");

                    if (loNhap.TrangThai != TrangThaiLoNhap.PENDING)
                        throw new InvalidBusinessRuleException("Chỉ được sửa phiếu khi đang ở trạng thái Chờ duyệt.");

                    loNhap.NhaCungCapId = dto.NhaCungCapId;
                    loNhap.GhiChu = dto.GhiChu;
                }

                if (dto.ChiTietIdsXoa.Count > 0)
                {
                    var canXoa = loNhap.ChiTiets.Where(x => dto.ChiTietIdsXoa.Contains(x.Id)).ToList();
                    _context.LoNhapChiTiet.RemoveRange(canXoa);
                    foreach (var x in canXoa) loNhap.ChiTiets.Remove(x);
                }

                if (dto.ChiTiets.Count == 0)
                    throw new InvalidBusinessRuleException("Lô Nhập phải có ít nhất 1 dòng chi tiết.");

                var sttTiepTheo = TinhSttTiepTheo(loNhap.ChiTiets);

                foreach (var ct in dto.ChiTiets)
                {
                    if (ct.SoLuongNhap <= 0)
                        throw new InvalidBusinessRuleException("Số lượng nhập phải lớn hơn 0.");

                    if (string.IsNullOrWhiteSpace(ct.TenVatPham))
                        throw new InvalidBusinessRuleException("Tên vật phẩm không được để trống.");

                    var hangHoa = await _context.HangHoa
                        .FirstOrDefaultAsync(x => x.Id == ct.HangHoaId && x.IsActive)
                        ?? throw new InvalidBusinessRuleException("Không tìm thấy Hàng Hóa hoặc hàng hóa đã ngừng sử dụng.");

                    if (ct.LoaiTaiSanId <= 0)
                        throw new InvalidBusinessRuleException("Bắt buộc chọn Loại Tài Sản cho dòng chi tiết.");

                    var loaiTaiSan = await _context.LoaiTaiSan.FindAsync(ct.LoaiTaiSanId)
                        ?? throw new InvalidBusinessRuleException("Không tìm thấy Loại Tài Sản đã chọn.");

                    if (ct.DonGia!=0 && (ct.DonGia < loaiTaiSan.MinValue || ct.DonGia > loaiTaiSan.MaxValue))
                        throw new InvalidBusinessRuleException(
                            $"Đơn giá {ct.DonGia:N0} không nằm trong ngưỡng của '{loaiTaiSan.Name}' " +
                            $"({loaiTaiSan.MinValue:N0} - {loaiTaiSan.MaxValue:N0}).");

                    if (ct.Id == null)
                    {
                        _context.LoNhapChiTiet.Add(new LoNhapChiTiet
                        {
                            LoNhapId = loNhap.Id,
                            SoLo = $"{loNhap.SoLo}-{sttTiepTheo:00}",
                            HangHoaId = ct.HangHoaId,
                            TenVatPham = ct.TenVatPham,
                            SoLuongNhap = ct.SoLuongNhap,
                            DonGia = ct.DonGia,
                            LoaiTaiSanId = ct.LoaiTaiSanId
                        });
                        sttTiepTheo++;
                    }
                    else
                    {
                        var chiTietCu = loNhap.ChiTiets.FirstOrDefault(x => x.Id == ct.Id.Value)
                            ?? throw new InvalidBusinessRuleException("Không tìm thấy dòng chi tiết cần cập nhật.");

                        chiTietCu.HangHoaId = ct.HangHoaId;
                        chiTietCu.TenVatPham = ct.TenVatPham;
                        chiTietCu.SoLuongNhap = ct.SoLuongNhap;
                        chiTietCu.DonGia = ct.DonGia;
                        chiTietCu.LoaiTaiSanId = ct.LoaiTaiSanId;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return loNhap.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                _context.ChangeTracker.Clear();   // ⬅ thêm dòng này — xóa mọi entity đang track sau khi rollback
                throw;
            }
        }

        public async Task DuyetLoNhapAsync(long loNhapId, long nguoiDuyetId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var loNhap = await _context.LoNhap
                    .Include(x => x.ChiTiets)
                        .ThenInclude(ct => ct.HangHoa)
                            .ThenInclude(hh => hh.DMTaiSan)   // thêm dòng này
                    .Include(x => x.ChiTiets)
                        .ThenInclude(ct => ct.LoaiTaiSan)
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

                // Lưu kèm mã danh mục để dùng khi cập nhật Code ở bước 2 (sau khi có Id).
                var taiSanMoiTao = new List<(TaiSanDinhDanh TaiSan, string MaDanhMuc)>();

                foreach (var chiTiet in loNhap.ChiTiets)
                {
                    if (chiTiet.LoaiTaiSan.Code != MA_LOAI_VAT_TU)
                    {
                        var maDanhMuc = chiTiet.HangHoa.DMTaiSan.Code;

                        for (var i = 0; i < chiTiet.SoLuongNhap; i++)
                        {
                            var taiSan = new TaiSanDinhDanh
                            {
                                Name = chiTiet.TenVatPham,
                                Code = $"TMP-{Guid.NewGuid():N}",
                                HangHoaId = chiTiet.HangHoaId,
                                Serial = null,
                                NamSuDung = loNhap.NgayNhap.Year,
                                GiaNhap = chiTiet.DonGia,
                                LoaiTaiSanId = chiTiet.LoaiTaiSanId,
                                TrangThaiTaiSan = TrangThaiTaiSan.IN_STOCK,
                                LoNhapChiTietId = chiTiet.Id,
                                ViTriTaiSanId = khoNhap.Id
                            };

                            _context.TaiSanDinhDanh.Add(taiSan);
                            taiSanMoiTao.Add((taiSan, maDanhMuc));
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
                                Name = chiTiet.TenVatPham,
                                HangHoaId = chiTiet.HangHoaId,
                                SoLuongTon = 0,
                                ViTriTaiSanId = khoNhap.Id,
                                DonViTinh= chiTiet.HangHoa.DonViTinh
                            };
                            _context.VatTu.Add(vatTu);
                        }

                        vatTu.SoLuongTon += chiTiet.SoLuongNhap;
                    }
                }

                loNhap.TrangThai = TrangThaiLoNhap.APPROVED;
                loNhap.NguoiDuyetId = nguoiDuyetId;
                await _context.SaveChangesAsync();

                foreach (var (taiSan, maDanhMuc) in taiSanMoiTao)
                {
                    taiSan.Code = $"TS-{maDanhMuc}-{taiSan.Id:D6}";
                }

                if (taiSanMoiTao.Count > 0)
                    await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                _context.ChangeTracker.Clear();
                throw;
            }
        }

        public async Task TuChoiLoNhapAsync(long loNhapId, long nguoiDuyetId)
        {
            var loNhap = await _context.LoNhap.FindAsync(loNhapId)
                ?? throw new InvalidBusinessRuleException("Không tìm thấy Lô Nhập.");

            if (loNhap.TrangThai != TrangThaiLoNhap.PENDING)
                throw new InvalidBusinessRuleException(
                    "Chỉ được từ chối phiếu khi đang ở trạng thái Chờ duyệt — phiếu đã duyệt đã sinh tài sản/vật tư, không thể hủy trực tiếp.");

            loNhap.TrangThai = TrangThaiLoNhap.REJECTED;
            loNhap.NguoiDuyetId = nguoiDuyetId;   // ⬅ mới
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
                .Include(x => x.NguoiDuyet)              // ⬅ mới
                .Include(x => x.ChiTiets)
                    .ThenInclude(ct => ct.HangHoa)
                .Include(x => x.ChiTiets)
                    .ThenInclude(ct => ct.LoaiTaiSan)
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
        public async Task DeleteAsync(long loNhapId)
        {
            var loNhap = await _context.LoNhap
                .Include(x => x.ChiTiets)
                .FirstOrDefaultAsync(x => x.Id == loNhapId)
                ?? throw new InvalidBusinessRuleException("Không tìm thấy Lô Nhập.");

            if (loNhap.TrangThai != TrangThaiLoNhap.PENDING)
                throw new InvalidBusinessRuleException("Chỉ được xóa phiếu đang ở trạng thái Chờ Duyệt (PENDING).");
            try
            {
                _context.LoNhap.Remove(loNhap);
                await _context.SaveChangesAsync();
            }
            catch
            {
                _context.ChangeTracker.Clear();   // dọn sạch entity bị đánh dấu Deleted khi SaveChanges thất bại
                throw;
            }
        }

        // ===================== HELPER DÙNG CHUNG =====================

        /// <summary>STT dòng chi tiết tiếp theo = MAX STT hiện có + 1 (không dùng Count — tránh trùng khi có dòng bị xóa giữa chừng).</summary>
        private static int TinhSttTiepTheo(IEnumerable<LoNhapChiTiet> chiTietHienCo)
        {
            return chiTietHienCo
                .Select(ct =>
                {
                    var suffix = ct.SoLo.Contains('-') ? ct.SoLo[(ct.SoLo.LastIndexOf('-') + 1)..] : null;
                    return int.TryParse(suffix, out var n) ? n : 0;
                })
                .DefaultIfEmpty(0)
                .Max() + 1;
        }

        /// <summary>Sinh số phiếu dạng PN-{năm}-{stt:D4}, STT reset mỗi năm, tính theo MAX STT hiện có trong năm + 1.</summary>
        private async Task<string> TinhSoPhieuMoiAsync(long loNhapIdMoiTao, DateTime ngayNhap)
        {
            var nam = ngayNhap.Year;
            var prefix = $"PN-{nam}-";

            var soLoTrongNam = await _context.LoNhap
                .Where(x => x.Id != loNhapIdMoiTao && x.SoLo.StartsWith(prefix))
                .Select(x => x.SoLo)
                .ToListAsync();

            var sttLonNhat = soLoTrongNam
                .Select(soLo =>
                {
                    var suffix = soLo[prefix.Length..];
                    return int.TryParse(suffix, out var n) ? n : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            var stt = sttLonNhat + 1;
            if (stt > 9999)
                throw new InvalidBusinessRuleException($"Đã vượt quá số lượng phiếu nhập tối đa (9999) trong năm {nam}.");

            return $"{prefix}{stt:D4}";
        }
    }
}