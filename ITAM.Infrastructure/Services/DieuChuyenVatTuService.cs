using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities;
using ITAM.Domain.Enums;
using ITAM.Domain.Exceptions;
using ITAM.Infrastructure.Data;
using ITAM.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITAM.Infrastructure.Services
{
    public class DieuChuyenVatTuService : IDieuChuyenVatTuService
    {
        private readonly AppDbContext _context;

        public DieuChuyenVatTuService(AppDbContext context)
        {
            _context = context;
        }

        // ------------------------------------------------------------------
        // Tồn khả dụng
        // ------------------------------------------------------------------

        public async Task<List<VatTuKhaDungDto>> GetVatTuKhaDungAsync(long phongBanId, long? boQuaPhieuId = null)
        {
            var phongBan = await _context.PhongBan.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == phongBanId);

            if (phongBan == null || VatTuKhoRules.PhongBanBiChan.Contains(phongBan.Code))
                return new List<VatTuKhaDungDto>();

            var boQua = boQuaPhieuId ?? 0;

            var tons = await _context.VatTu.AsNoTracking()
                .Where(v => v.ViTriTaiSan.PhongBanId == phongBanId && v.SoLuongTon > 0)
                .Select(v => new VatTuKhaDungDto
                {
                    HangHoaId = v.HangHoaId,
                    MaHangHoa = v.HangHoa.Code,
                    TenHangHoa = v.HangHoa.Name,            // TODO xác nhận: HangHoa có Name?
                    ViTriTaiSanId = v.ViTriTaiSanId,
                    TenViTri = v.ViTriTaiSan.Name,
                    SoLuongTon = v.SoLuongTon
                })
                .ToListAsync();

            // Số lượng đang bị giữ bởi các phiếu PENDING khác (phiếu đang sửa không tính)
            var daGiu = await _context.DieuChuyenVatTuChiTiet.AsNoTracking()
                .Where(c => c.DieuChuyenVatTu.TrangThai == TrangThaiPhieu.PENDING
                         && c.DieuChuyenVatTuId != boQua
                         && c.ViTriChuyenDi.PhongBanId == phongBanId)
                .GroupBy(c => new { c.HangHoaId, c.ViTriChuyenDiId })
                .Select(g => new { g.Key.HangHoaId, ViTriId = g.Key.ViTriChuyenDiId, Tong = g.Sum(x => x.SoLuong) })
                .ToListAsync();

            foreach (var t in tons)
            {
                var giu = daGiu.FirstOrDefault(x => x.HangHoaId == t.HangHoaId && x.ViTriId == t.ViTriTaiSanId)?.Tong ?? 0;
                t.TonKhaDung = t.SoLuongTon - giu;
            }

            return tons.Where(t => t.TonKhaDung > 0)
                       .OrderBy(t => t.MaHangHoa)
                       .ThenBy(t => t.TenViTri)
                       .ToList();
        }

        // ------------------------------------------------------------------
        // Lưu nháp
        // ------------------------------------------------------------------

        public async Task<long> LuuPhieuAsync(SaveDieuChuyenVatTuDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                DieuChuyenVatTu phieu;

                if (dto.Id is long id && id > 0)
                {
                    phieu = await _context.DieuChuyenVatTu
                        .Include(x => x.ChiTiets)
                        .FirstOrDefaultAsync(x => x.Id == id)
                        ?? throw new InvalidBusinessRuleException("Không tìm thấy Phiếu Điều Chuyển Vật Tư.");

                    if (phieu.TrangThai != TrangThaiPhieu.PENDING)
                        throw new InvalidBusinessRuleException("Phiếu đã được duyệt hoặc đã từ chối, không thể sửa.");
                }
                else
                {
                    phieu = new DieuChuyenVatTu
                    {
                        SoPhieu = await SinhSoPhieuAsync(),
                        NgayTao = DateTime.Now,
                        NguoiTaoId = dto.NguoiTaoId,
                        // Phòng Ban Chuyển Đi cố định = Phòng Ban mặc định của người tạo (ViewModel gán sẵn trong dto)
                        PhongBanChuyenDiId = dto.PhongBanChuyenDiId,
                        TrangThai = TrangThaiPhieu.PENDING
                    };
                    _context.DieuChuyenVatTu.Add(phieu);
                }

                if (dto.PhongBanChuyenDenId <= 0)
                    throw new InvalidBusinessRuleException("Chưa chọn Phòng Ban Chuyển Đến.");

                var phongBans = await _context.PhongBan.AsNoTracking()
                    .Where(x => x.Id == phieu.PhongBanChuyenDiId || x.Id == dto.PhongBanChuyenDenId)
                    .ToDictionaryAsync(x => x.Id);

                var pbDi = phongBans.GetValueOrDefault(phieu.PhongBanChuyenDiId)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Phòng Ban Chuyển Đi.");
                var pbDen = phongBans.GetValueOrDefault(dto.PhongBanChuyenDenId)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Phòng Ban Chuyển Đến.");

                if (VatTuKhoRules.PhongBanBiChan.Contains(pbDi.Code))
                    throw new InvalidBusinessRuleException($"Vật tư không được điều chuyển đi từ '{pbDi.Name}'.");
                if (VatTuKhoRules.PhongBanBiChan.Contains(pbDen.Code))
                    throw new InvalidBusinessRuleException($"Vật tư không được điều chuyển đến '{pbDen.Name}'.");

                // Đổi Phòng Ban Chuyển Đến khi đã có dòng chi tiết sẽ làm các Vị Trí Chuyển Đến cũ trỏ sai phòng ban
                if (phieu.Id != 0 && phieu.PhongBanChuyenDenId != 0 && phieu.PhongBanChuyenDenId != dto.PhongBanChuyenDenId
                    && phieu.ChiTiets.Any(x => !dto.ChiTietIdsXoa.Contains(x.Id)))
                    throw new InvalidBusinessRuleException(
                        "Không thể đổi Phòng Ban Chuyển Đến khi phiếu đã có dòng chi tiết — xóa hết dòng chi tiết trước khi đổi.");

                phieu.PhongBanChuyenDenId = dto.PhongBanChuyenDenId;
                phieu.NguoiGiao = dto.NguoiGiao;
                phieu.NguoiNhan = dto.NguoiNhan;
                phieu.GhiChu = dto.GhiChu;

                // Dòng đã có: HangHoaId/ViTriChuyenDiId là snapshot trong DB, không tin dữ liệu client.
                // Ghi đè lại vào dto để mọi kiểm tra bên dưới chạy trên đúng cặp sẽ bị trừ khi duyệt.
                foreach (var ctDto in dto.ChiTiets.Where(x => x.Id is long i && i > 0))
                {
                    var hienCo = phieu.ChiTiets.FirstOrDefault(x => x.Id == ctDto.Id)
                        ?? throw new InvalidBusinessRuleException("Không tìm thấy dòng chi tiết cần cập nhật.");
                    ctDto.HangHoaId = hienCo.HangHoaId;
                    ctDto.ViTriChuyenDiId = hienCo.ViTriChuyenDiId;
                }

                if (dto.ChiTiets.GroupBy(x => new { x.HangHoaId, x.ViTriChuyenDiId }).Any(g => g.Count() > 1))
                    throw new InvalidBusinessRuleException(
                        "Một vật tư tại cùng 1 vị trí chỉ được xuất hiện 1 dòng trong phiếu.");

                // Đánh dấu Deleted nhưng collection vẫn còn các dòng này đến khi SaveChanges,
                // nên bước đếm số dòng còn lại bên dưới phải loại trừ các Id này.
                foreach (var xoaId in dto.ChiTietIdsXoa)
                {
                    var ct = phieu.ChiTiets.FirstOrDefault(x => x.Id == xoaId);
                    if (ct != null) _context.DieuChuyenVatTuChiTiet.Remove(ct);
                }

                foreach (var ctDto in dto.ChiTiets)
                {
                    await KiemTraChiTietHopLeAsync(ctDto, phieu.Id, phieu.PhongBanChuyenDiId, phieu.PhongBanChuyenDenId);

                    if (ctDto.Id is long ctId && ctId > 0)
                    {
                        var ct = phieu.ChiTiets.First(x => x.Id == ctId);
                        // Chỉ cho sửa Vị Trí Đến, Số lượng, Ghi chú
                        ct.ViTriChuyenDenId = ctDto.ViTriChuyenDenId;
                        ct.SoLuong = ctDto.SoLuong;
                        ct.GhiChu = ctDto.GhiChu;
                    }
                    else
                    {
                        // Add qua navigation để EF tự fixup FK (phieu.Id = 0 khi tạo mới)
                        phieu.ChiTiets.Add(new DieuChuyenVatTuChiTiet
                        {
                            HangHoaId = ctDto.HangHoaId,
                            ViTriChuyenDiId = ctDto.ViTriChuyenDiId,
                            ViTriChuyenDenId = ctDto.ViTriChuyenDenId,
                            SoLuong = ctDto.SoLuong,
                            GhiChu = ctDto.GhiChu
                        });
                    }
                }

                if (phieu.ChiTiets.Count(x => !dto.ChiTietIdsXoa.Contains(x.Id)) == 0)
                    throw new InvalidBusinessRuleException("Phiếu điều chuyển phải có ít nhất 1 vật tư.");

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return phieu.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                _context.ChangeTracker.Clear();
                throw;
            }
        }

        /// <summary>
        /// Kiểm tra 1 dòng chi tiết khi Lưu nháp: tính cả số lượng các phiếu PENDING khác đang giữ.
        /// Lúc Duyệt kiểm tra lại riêng (chỉ cần tồn thực đủ) vì tồn có thể đã đổi từ lúc lập phiếu.
        /// </summary>
        private async Task KiemTraChiTietHopLeAsync(SaveDieuChuyenVatTuChiTietDto ct, long phieuIdHienTai,
            long phongBanChuyenDiId, long phongBanChuyenDenId)
        {
            if (ct.SoLuong <= 0)
                throw new InvalidBusinessRuleException("Số lượng phải lớn hơn 0.");

            var viTriDi = await _context.ViTriTaiSan.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == ct.ViTriChuyenDiId)
                ?? throw new InvalidBusinessRuleException("Không tìm thấy Vị Trí Chuyển Đi.");

            var viTriDen = await _context.ViTriTaiSan.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == ct.ViTriChuyenDenId)
                ?? throw new InvalidBusinessRuleException("Không tìm thấy Vị Trí Chuyển Đến.");

            if (viTriDi.PhongBanId != phongBanChuyenDiId)
                throw new InvalidBusinessRuleException("Vị Trí Chuyển Đi không thuộc Phòng Ban Chuyển Đi của phiếu.");

            if (viTriDen.PhongBanId != phongBanChuyenDenId)
                throw new InvalidBusinessRuleException("Vị Trí Chuyển Đến không thuộc Phòng Ban Chuyển Đến của phiếu.");

            if (VatTuKhoRules.ViTriBiChan.Contains(viTriDi.Code) || VatTuKhoRules.ViTriBiChan.Contains(viTriDen.Code))
                throw new InvalidBusinessRuleException(
                    "Vật tư không được điều chuyển đi/đến Kho Thất Lạc, Kho Chờ Thanh Lý, Kho Thanh Lý.");

            if (ct.ViTriChuyenDiId == ct.ViTriChuyenDenId)
                throw new InvalidBusinessRuleException("Vị Trí Chuyển Đi và Vị Trí Chuyển Đến không được trùng nhau.");

            var ton = await _context.VatTu.AsNoTracking()
                .Where(v => v.HangHoaId == ct.HangHoaId && v.ViTriTaiSanId == ct.ViTriChuyenDiId)
                .Select(v => (int?)v.SoLuongTon)
                .FirstOrDefaultAsync() ?? 0;

            var daGiu = await _context.DieuChuyenVatTuChiTiet.AsNoTracking()
                .Where(x => x.DieuChuyenVatTuId != phieuIdHienTai
                         && x.DieuChuyenVatTu.TrangThai == TrangThaiPhieu.PENDING
                         && x.HangHoaId == ct.HangHoaId
                         && x.ViTriChuyenDiId == ct.ViTriChuyenDiId)
                .SumAsync(x => (int?)x.SoLuong) ?? 0;

            var khaDung = Math.Max(ton - daGiu, 0);
            if (ct.SoLuong > khaDung)
                throw new InvalidBusinessRuleException(
                    $"Không đủ tồn khả dụng tại '{viTriDi.Name}': còn {khaDung}, yêu cầu {ct.SoLuong}.");
        }

        // ------------------------------------------------------------------
        // Duyệt / Từ chối / Xóa
        // ------------------------------------------------------------------

        public async Task DuyetPhieuAsync(long dieuChuyenVatTuId, long nguoiDuyetId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var phieu = await _context.DieuChuyenVatTu
                    .Include(x => x.ChiTiets).ThenInclude(ct => ct.ViTriChuyenDi)
                    .Include(x => x.ChiTiets).ThenInclude(ct => ct.ViTriChuyenDen)
                    .FirstOrDefaultAsync(x => x.Id == dieuChuyenVatTuId)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Phiếu Điều Chuyển Vật Tư.");

                if (phieu.TrangThai != TrangThaiPhieu.PENDING)
                    throw new InvalidBusinessRuleException("Phiếu đã được duyệt hoặc đã từ chối, không thể duyệt lại.");

                if (!phieu.ChiTiets.Any())
                    throw new InvalidBusinessRuleException("Phiếu điều chuyển chưa có dòng chi tiết nào.");

                var thoiDiemDuyet = DateTime.Now;

                foreach (var ct in phieu.ChiTiets)
                {
                    // Re-check tại thời điểm duyệt: tồn thực tế có thể đã đổi kể từ lúc lập phiếu.
                    // Các phiếu PENDING khác chưa trừ gì nên chỉ cần tồn thực đủ, không tính phần "giữ chỗ".
                    if (ct.ViTriChuyenDiId == ct.ViTriChuyenDenId)
                        throw new InvalidBusinessRuleException(
                            "Vị Trí Chuyển Đi và Vị Trí Chuyển Đến không được trùng nhau.");

                    if (VatTuKhoRules.ViTriBiChan.Contains(ct.ViTriChuyenDi.Code) ||
                        VatTuKhoRules.ViTriBiChan.Contains(ct.ViTriChuyenDen.Code))
                        throw new InvalidBusinessRuleException(
                            "Vật tư không được điều chuyển đi/đến Kho Thất Lạc, Kho Chờ Thanh Lý, Kho Thanh Lý.");

                    var nguon = await TimVatTuAsync(ct.HangHoaId, ct.ViTriChuyenDiId);
                    if (nguon == null || nguon.SoLuongTon < ct.SoLuong)
                        throw new InvalidBusinessRuleException(
                            $"Không đủ tồn tại '{ct.ViTriChuyenDi.Name}' để duyệt: còn {nguon?.SoLuongTon ?? 0}, cần {ct.SoLuong}.");

                    var dich = await TimVatTuAsync(ct.HangHoaId, ct.ViTriChuyenDenId);
                    if (dich == null)
                    {
                        dich = new VatTu
                        {
                            HangHoaId = ct.HangHoaId,
                            ViTriTaiSanId = ct.ViTriChuyenDenId,
                            SoLuongTon = 0
                        };
                        _context.VatTu.Add(dich);
                    }

                    nguon.SoLuongTon -= ct.SoLuong;     // giữ lại dòng VatTu nguồn dù tồn về 0
                    dich.SoLuongTon += ct.SoLuong;

                    _context.LichSuBienDongVatTu.AddRange(
                        new LichSuBienDongVatTu
                        {
                            HangHoaId = ct.HangHoaId,
                            ViTriTaiSanId = ct.ViTriChuyenDiId,
                            Loai = LoaiBienDongVatTu.CHUYEN_DI,
                            SoLuong = ct.SoLuong,
                            TonSau = nguon.SoLuongTon,
                            DieuChuyenVatTuChiTietId = ct.Id,
                            NguoiDuyetId = nguoiDuyetId,
                            NgayDuyet = thoiDiemDuyet,
                            GhiChu = ct.GhiChu
                        },
                        new LichSuBienDongVatTu
                        {
                            HangHoaId = ct.HangHoaId,
                            ViTriTaiSanId = ct.ViTriChuyenDenId,
                            Loai = LoaiBienDongVatTu.CHUYEN_DEN,
                            SoLuong = ct.SoLuong,
                            TonSau = dich.SoLuongTon,
                            DieuChuyenVatTuChiTietId = ct.Id,
                            NguoiDuyetId = nguoiDuyetId,
                            NgayDuyet = thoiDiemDuyet,
                            GhiChu = ct.GhiChu
                        });
                }

                phieu.TrangThai = TrangThaiPhieu.APPROVED;
                phieu.NguoiDuyetId = nguoiDuyetId;
                phieu.NgayDuyet = thoiDiemDuyet;

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

        /// <summary>
        /// Tìm dòng VatTu theo (Hàng hóa, Vị trí). Tìm trong Local trước vì bản ghi có thể vừa được Add
        /// ở dòng chi tiết trước trong cùng phiếu nhưng chưa SaveChanges (cả nguồn lẫn đích đều có thể gặp).
        /// </summary>
        private async Task<VatTu?> TimVatTuAsync(long hangHoaId, long viTriId)
        {
            return _context.VatTu.Local.FirstOrDefault(v => v.HangHoaId == hangHoaId && v.ViTriTaiSanId == viTriId)
                   ?? await _context.VatTu.FirstOrDefaultAsync(v => v.HangHoaId == hangHoaId && v.ViTriTaiSanId == viTriId);
        }

        public async Task TuChoiPhieuAsync(long dieuChuyenVatTuId, long nguoiDuyetId)
        {
            try
            {
                var phieu = await _context.DieuChuyenVatTu.FirstOrDefaultAsync(x => x.Id == dieuChuyenVatTuId)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Phiếu Điều Chuyển Vật Tư.");

                if (phieu.TrangThai != TrangThaiPhieu.PENDING)
                    throw new InvalidBusinessRuleException("Phiếu đã được duyệt hoặc đã từ chối, không thể từ chối lại.");

                phieu.TrangThai = TrangThaiPhieu.REJECTED;
                phieu.NguoiDuyetId = nguoiDuyetId;
                phieu.NgayDuyet = DateTime.Now;

                await _context.SaveChangesAsync();
            }
            catch
            {
                _context.ChangeTracker.Clear();
                throw;
            }
        }

        public async Task DeleteAsync(long dieuChuyenVatTuId)
        {
            try
            {
                var phieu = await _context.DieuChuyenVatTu
                    .Include(x => x.ChiTiets)
                    .FirstOrDefaultAsync(x => x.Id == dieuChuyenVatTuId)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Phiếu Điều Chuyển Vật Tư.");

                if (phieu.TrangThai != TrangThaiPhieu.PENDING)
                    throw new InvalidBusinessRuleException("Chỉ được xóa phiếu đang ở trạng thái Chờ duyệt.");

                _context.DieuChuyenVatTu.Remove(phieu);   // Cascade xóa ChiTiets theo cấu hình
                await _context.SaveChangesAsync();
            }
            catch
            {
                _context.ChangeTracker.Clear();
                throw;
            }
        }

        // ------------------------------------------------------------------
        // Truy vấn
        // ------------------------------------------------------------------

        public async Task<DieuChuyenVatTu?> GetByIdAsync(long dieuChuyenVatTuId)
        {
            return await _context.DieuChuyenVatTu
                .Include(x => x.NguoiTao)
                .Include(x => x.NguoiDuyet)
                .Include(x => x.PhongBanChuyenDi)
                .Include(x => x.PhongBanChuyenDen)
                .Include(x => x.ChiTiets).ThenInclude(ct => ct.HangHoa)
                .Include(x => x.ChiTiets).ThenInclude(ct => ct.ViTriChuyenDi)
                .Include(x => x.ChiTiets).ThenInclude(ct => ct.ViTriChuyenDen)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == dieuChuyenVatTuId);
        }

        public async Task<List<DieuChuyenVatTu>> TimKiemAsync(DieuChuyenVatTuSearchDto dto)
        {
            var query = _context.DieuChuyenVatTu
                .Include(x => x.NguoiTao)
                .Include(x => x.PhongBanChuyenDi)
                .Include(x => x.PhongBanChuyenDen)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.SoPhieu))
                query = query.Where(x => x.SoPhieu.Contains(dto.SoPhieu));
            if (dto.TuNgay.HasValue)
                query = query.Where(x => x.NgayTao >= dto.TuNgay.Value.Date);
            if (dto.DenNgay.HasValue)
                // NgayTao có cả giờ, DenNgay thường là 00:00 → so sánh nhỏ hơn đầu ngày kế tiếp để không mất phiếu ngày cuối
                query = query.Where(x => x.NgayTao < dto.DenNgay.Value.Date.AddDays(1));
            if (dto.TrangThai.HasValue)
                query = query.Where(x => x.TrangThai == dto.TrangThai.Value);

            return await query.OrderByDescending(x => x.NgayTao).ToListAsync();
        }

        /// <summary>Format DCVT-{năm}-{stt:D4}, reset theo năm. STT = MAX + 1 trong năm hiện tại (không dùng COUNT).</summary>
        private async Task<string> SinhSoPhieuAsync()
        {
            var prefix = $"DCVT-{DateTime.Now.Year}-";

            var soPhieuTrongNam = await _context.DieuChuyenVatTu
                .Where(x => x.SoPhieu.StartsWith(prefix))
                .Select(x => x.SoPhieu)
                .ToListAsync();

            var stt = soPhieuTrongNam
                .Select(x => int.TryParse(x.Substring(prefix.Length), out var n) ? n : 0)
                .DefaultIfEmpty(0)
                .Max() + 1;

            return $"{prefix}{stt:D4}";
        }
    }
}