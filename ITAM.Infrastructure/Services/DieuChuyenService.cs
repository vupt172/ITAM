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
    public class DieuChuyenService : IDieuChuyenService
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Các Code Vị Trí KHÔNG được phép chọn làm Vị Trí Chuyển Đến qua màn hình Điều Chuyển thường —
        /// Kho Thất Lạc (có nghiệp vụ "báo mất" riêng, chưa xây) và Kho Đã Thanh Lý (có chức năng Thanh Lý
        /// riêng, chưa xây). Kho Chờ Thanh Lý (DISPOSAL_PENDING) KHÔNG bị chặn — vẫn điều chuyển được qua đây.
        /// </summary>
        private static readonly string[] MA_VI_TRI_CHAN_DIEU_CHUYEN_DEN =
        {
            ViTriTaiSanCodes.KHO_THAT_LAC,
            ViTriTaiSanCodes.KHO_THANH_LY
        };

        /// <summary>
        /// Các Phòng Ban hệ thống KHÔNG được phép chọn làm Phòng Ban Chuyển Đến — mỗi phòng ban này có
        /// đúng 1 ViTriTaiSan tương ứng nằm trong MA_VI_TRI_CHAN_DIEU_CHUYEN_DEN ở trên, nên chặn ngay ở
        /// đây để báo lỗi sớm (không cần đợi validate từng dòng chi tiết mới biết sai).
        /// </summary>
        private static readonly string[] MA_PHONG_BAN_CHAN_DIEU_CHUYEN_DEN =
        {
            PhongBanCodes.KHO_THAT_LAC,
            PhongBanCodes.KHO_THANH_LY
        };

        public DieuChuyenService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<long> LuuPhieuAsync(SaveDieuChuyenDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                DieuChuyen phieu;

                if (dto.Id is long id && id > 0)
                {
                    phieu = await _context.DieuChuyen
                        .Include(x => x.ChiTiets)
                        .FirstOrDefaultAsync(x => x.Id == id)
                        ?? throw new InvalidBusinessRuleException("Không tìm thấy Phiếu Điều Chuyển.");

                    if (phieu.TrangThai != TrangThaiDieuChuyen.PENDING)
                        throw new InvalidBusinessRuleException("Phiếu đã được duyệt hoặc đã từ chối, không thể sửa.");
                }
                else
                {
                    phieu = new DieuChuyen
                    {
                        SoPhieu = await SinhSoPhieuAsync(),
                        NgayTao = DateTime.Now,
                        NguoiTaoId = dto.NguoiTaoId,
                        // Phòng Ban Chuyển Đi cố định = Phòng Ban mặc định của người tạo, do ViewModel gán sẵn trong dto — không cho sửa sau khi tạo.
                        PhongBanChuyenDiId = dto.PhongBanChuyenDiId,
                        TrangThai = TrangThaiDieuChuyen.PENDING
                    };
                    _context.DieuChuyen.Add(phieu);
                }

                if (dto.PhongBanChuyenDenId <= 0)
                    throw new InvalidBusinessRuleException("Chưa chọn Phòng Ban Chuyển Đến.");

                // Chặn sớm ở mức Phòng Ban — không cần đợi validate từng dòng chi tiết mới báo lỗi,
                // vì Kho Thất Lạc/Kho Đã Thanh Lý mỗi phòng ban chỉ có đúng 1 Vị Trí (chắc chắn mọi dòng đều sai).
                var phongBanDen = await _context.PhongBan.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == dto.PhongBanChuyenDenId)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Phòng Ban Chuyển Đến.");

                if (MA_PHONG_BAN_CHAN_DIEU_CHUYEN_DEN.Contains(phongBanDen.Code))
                    throw new InvalidBusinessRuleException(
                        $"Không thể điều chuyển tài sản tới Phòng Ban '{phongBanDen.Name}' qua chức năng này.");

                // Đổi Phòng Ban Chuyển Đến khi phiếu đang sửa (còn PENDING) sẽ làm sai lệch Vị Trí Chuyển Đến
                // của các dòng chi tiết đã thêm trước đó (vốn được lọc theo Phòng Ban Chuyển Đến cũ) — chặn ở
                // đây để không phát sinh dữ liệu chi tiết trỏ sai phòng ban.
                if (phieu.Id != 0 && phieu.PhongBanChuyenDenId != 0 && phieu.PhongBanChuyenDenId != dto.PhongBanChuyenDenId
                    && phieu.ChiTiets.Any(x => !dto.ChiTietIdsXoa.Contains(x.Id)))
                    throw new InvalidBusinessRuleException(
                        "Không thể đổi Phòng Ban Chuyển Đến khi phiếu đã có dòng chi tiết — xóa hết dòng chi tiết trước khi đổi.");

                phieu.PhongBanChuyenDenId = dto.PhongBanChuyenDenId;
                phieu.NguoiGiao = dto.NguoiGiao;
                phieu.NguoiNhan = dto.NguoiNhan;
                phieu.GhiChu = dto.GhiChu;

                // Xóa các dòng chi tiết được yêu cầu xóa (đánh dấu Deleted, chưa mất khỏi navigation collection
                // cho tới khi SaveChanges — nên bước đếm số dòng còn lại bên dưới vẫn phải loại trừ các Id này).
                foreach (var xoaId in dto.ChiTietIdsXoa)
                {
                    var ct = phieu.ChiTiets.FirstOrDefault(x => x.Id == xoaId);
                    if (ct != null) _context.DieuChuyenChiTiet.Remove(ct);
                }

                foreach (var ctDto in dto.ChiTiets)
                {
                    await KiemTraChiTietHopLeAsync(ctDto, phieu.Id, phieu.PhongBanChuyenDiId, phieu.PhongBanChuyenDenId);

                    if (ctDto.Id is long ctId && ctId > 0)
                    {
                        var ct = phieu.ChiTiets.FirstOrDefault(x => x.Id == ctId)
                            ?? throw new InvalidBusinessRuleException("Không tìm thấy dòng chi tiết cần cập nhật.");
                        // Chỉ cho sửa Vị Trí Chuyển Đến + Ghi chú — TaiSanDinhDanhId/ViTriChuyenDiId là snapshot
                        // tại thời điểm thêm dòng, không cho sửa tay sau đó.
                        ct.ViTriChuyenDenId = ctDto.ViTriChuyenDenId;
                        ct.GhiChu = ctDto.GhiChu;
                    }
                    else
                    {
                        // Add qua navigation collection (không set DieuChuyenId thủ công) để EF tự fixup
                        // FK lúc SaveChanges — cần thiết vì phieu.Id = 0 khi đang tạo phiếu mới.
                        phieu.ChiTiets.Add(new DieuChuyenChiTiet
                        {
                            TaiSanDinhDanhId = ctDto.TaiSanDinhDanhId,
                            ViTriChuyenDiId = ctDto.ViTriChuyenDiId,
                            ViTriChuyenDenId = ctDto.ViTriChuyenDenId,
                            GhiChu = ctDto.GhiChu
                        });
                    }
                }

                if (phieu.ChiTiets.Count(x => !dto.ChiTietIdsXoa.Contains(x.Id)) == 0)
                    throw new InvalidBusinessRuleException("Phiếu điều chuyển phải có ít nhất 1 tài sản.");

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
        /// Kiểm tra hợp lệ 1 dòng chi tiết trước khi thêm/sửa vào phiếu (Lưu nháp).
        /// Không kiểm tra lại khi Duyệt bằng hàm này — Duyệt có kiểm tra lại riêng (KiemTraChiTietLucDuyet)
        /// vì trạng thái tài sản có thể đã đổi kể từ lúc tạo phiếu tới lúc duyệt.
        /// </summary>
        private async Task KiemTraChiTietHopLeAsync(SaveDieuChuyenChiTietDto ctDto, long phieuIdHienTai,
            long phongBanChuyenDiId, long phongBanChuyenDenId)
        {
            var taiSan = await _context.TaiSanDinhDanh.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == ctDto.TaiSanDinhDanhId)
                ?? throw new InvalidBusinessRuleException("Không tìm thấy tài sản.");

            if (taiSan.TrangThaiTaiSan == TrangThaiTaiSan.MAINTENANCE)
                throw new InvalidBusinessRuleException($"Tài sản '{taiSan.Code}' đang sửa chữa, không thể điều chuyển.");

            if (taiSan.TrangThaiTaiSan == TrangThaiTaiSan.LOST)
                throw new InvalidBusinessRuleException($"Tài sản '{taiSan.Code}' đang Thất Lạc, không thể điều chuyển.");

            var viTriDi = await _context.ViTriTaiSan.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == ctDto.ViTriChuyenDiId)
                ?? throw new InvalidBusinessRuleException("Không tìm thấy Vị Trí Chuyển Đi.");

            if (viTriDi.PhongBanId != phongBanChuyenDiId)
                throw new InvalidBusinessRuleException(
                    $"Tài sản '{taiSan.Code}' không thuộc Phòng Ban Chuyển Đi của phiếu.");

            var viTriDen = await _context.ViTriTaiSan.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == ctDto.ViTriChuyenDenId)
                ?? throw new InvalidBusinessRuleException("Không tìm thấy Vị Trí Chuyển Đến.");

            if (viTriDen.PhongBanId != phongBanChuyenDenId)
                throw new InvalidBusinessRuleException(
                    $"Vị Trí Chuyển Đến của tài sản '{taiSan.Code}' không thuộc Phòng Ban Chuyển Đến của phiếu.");

            if (MA_VI_TRI_CHAN_DIEU_CHUYEN_DEN.Contains(viTriDen.Code))
                throw new InvalidBusinessRuleException(
                    $"Không thể điều chuyển tài sản tới '{viTriDen.Name}' qua chức năng này.");

            // Ghi chú: KHÔNG chặn ViTriChuyenDenId == ViTriChuyenDiId — cho phép điều chuyển nội bộ trong
            // cùng 1 Phòng Ban (PhongBanChuyenDi == PhongBanChuyenDen), kể cả trùng đúng 1 Vị Trí, vì mục
            // đích lúc đó chỉ là ghi nhận lại (VD: xác nhận tài sản vẫn ở đó, đổi người bàn giao).

            // Chặn 1 tài sản có mặt đồng thời trong 2 phiếu điều chuyển đang PENDING — tránh xung đột lịch sử
            // khi cả 2 phiếu cùng được duyệt. Đây là ràng buộc bổ sung ngoài yêu cầu ban đầu, có thể bỏ nếu
            // không cần thiết cho nghiệp vụ thực tế.
            var dangOPhieuKhac = await _context.DieuChuyenChiTiet.AsNoTracking()
                .AnyAsync(x => x.TaiSanDinhDanhId == ctDto.TaiSanDinhDanhId
                    && x.DieuChuyenId != phieuIdHienTai
                    && x.DieuChuyen.TrangThai == TrangThaiDieuChuyen.PENDING);

            if (dangOPhieuKhac)
                throw new InvalidBusinessRuleException($"Tài sản '{taiSan.Code}' đang có trong 1 Phiếu Điều Chuyển khác chưa được duyệt.");
        }

        public async Task DuyetPhieuAsync(long dieuChuyenId, long nguoiDuyetId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var phieu = await _context.DieuChuyen
                    .Include(x => x.ChiTiets)
                        .ThenInclude(ct => ct.TaiSanDinhDanh)
                    .Include(x => x.ChiTiets)
                        .ThenInclude(ct => ct.ViTriChuyenDen)
                    .FirstOrDefaultAsync(x => x.Id == dieuChuyenId)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Phiếu Điều Chuyển.");

                if (phieu.TrangThai != TrangThaiDieuChuyen.PENDING)
                    throw new InvalidBusinessRuleException("Phiếu đã được duyệt hoặc đã từ chối, không thể duyệt lại.");

                if (!phieu.ChiTiets.Any())
                    throw new InvalidBusinessRuleException("Phiếu điều chuyển chưa có dòng chi tiết nào.");

                var thoiDiemDuyet = DateTime.Now;

                foreach (var ct in phieu.ChiTiets)
                {
                    var taiSan = ct.TaiSanDinhDanh;

                    // Re-check tại thời điểm duyệt: trạng thái tài sản có thể đã đổi kể từ lúc tạo phiếu
                    // (VD: tài sản đã được đưa đi sửa chữa ở 1 luồng khác sau khi phiếu này được tạo).
                    if (taiSan.TrangThaiTaiSan == TrangThaiTaiSan.MAINTENANCE)
                        throw new InvalidBusinessRuleException($"Tài sản '{taiSan.Code}' đang sửa chữa, không thể duyệt điều chuyển.");

                    if (taiSan.TrangThaiTaiSan == TrangThaiTaiSan.LOST)
                        throw new InvalidBusinessRuleException($"Tài sản '{taiSan.Code}' đang Thất Lạc, không thể duyệt điều chuyển.");

                    var maViTriDen = ct.ViTriChuyenDen.Code;

                    if (MA_VI_TRI_CHAN_DIEU_CHUYEN_DEN.Contains(maViTriDen))
                        throw new InvalidBusinessRuleException(
                            $"Không thể điều chuyển tài sản '{taiSan.Code}' tới '{ct.ViTriChuyenDen.Name}' qua chức năng này.");

                    // Cập nhật TrangThaiTaiSan theo đích đến — chỉ áp dụng cho TaiSanDinhDanh (VatTu không có state machine).
                    if (maViTriDen == ViTriTaiSanCodes.KHO_LUU_TRU)
                        taiSan.TrangThaiTaiSan = TrangThaiTaiSan.IN_STOCK;
                    else if (maViTriDen == ViTriTaiSanCodes.KHO_CHO_THANH_LY)
                        taiSan.TrangThaiTaiSan = TrangThaiTaiSan.DISPOSAL_PENDING;
                    else
                        // Điều chuyển tới 1 Khoa/Phòng Ban chức năng bình thường (kể cả giữa 2 phòng ban khác nhau).
                        taiSan.TrangThaiTaiSan = TrangThaiTaiSan.ASSIGNED;

                    taiSan.ViTriTaiSanId = ct.ViTriChuyenDenId;

                    _context.LichSuDieuChuyenTaiSan.Add(new LichSuDieuChuyenTaiSan
                    {
                        TaiSanDinhDanhId = ct.TaiSanDinhDanhId,
                        ViTriChuyenDiId = ct.ViTriChuyenDiId,
                        ViTriChuyenDenId = ct.ViTriChuyenDenId,
                        NgayDuyet = thoiDiemDuyet,
                        NguoiDuyetId = nguoiDuyetId,
                        DieuChuyenChiTietId = ct.Id,
                        GhiChu = ct.GhiChu
                    });
                }

                phieu.TrangThai = TrangThaiDieuChuyen.APPROVED;
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

        public async Task TuChoiPhieuAsync(long dieuChuyenId, long nguoiDuyetId)
        {
            try
            {
                var phieu = await _context.DieuChuyen.FirstOrDefaultAsync(x => x.Id == dieuChuyenId)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Phiếu Điều Chuyển.");

                if (phieu.TrangThai != TrangThaiDieuChuyen.PENDING)
                    throw new InvalidBusinessRuleException("Phiếu đã được duyệt hoặc đã từ chối, không thể từ chối lại.");

                phieu.TrangThai = TrangThaiDieuChuyen.REJECTED;
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

        public async Task DeleteAsync(long dieuChuyenId)
        {
            try
            {
                var phieu = await _context.DieuChuyen
                    .Include(x => x.ChiTiets)
                    .FirstOrDefaultAsync(x => x.Id == dieuChuyenId)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Phiếu Điều Chuyển.");

                if (phieu.TrangThai != TrangThaiDieuChuyen.PENDING)
                    throw new InvalidBusinessRuleException("Chỉ được xóa phiếu đang ở trạng thái Chờ duyệt.");

                _context.DieuChuyen.Remove(phieu); // Cascade xóa ChiTiets theo cấu hình DieuChuyenConfiguration.
                await _context.SaveChangesAsync();
            }
            catch
            {
                _context.ChangeTracker.Clear();
                throw;
            }
        }

        public async Task<DieuChuyen?> GetByIdAsync(long dieuChuyenId)
        {
            return await _context.DieuChuyen
                .Include(x => x.NguoiTao)
                .Include(x => x.NguoiDuyet)
                .Include(x => x.PhongBanChuyenDi)
                .Include(x => x.PhongBanChuyenDen)
                .Include(x => x.ChiTiets)
                    .ThenInclude(ct => ct.TaiSanDinhDanh)
                .Include(x => x.ChiTiets)
                    .ThenInclude(ct => ct.ViTriChuyenDi)
                .Include(x => x.ChiTiets)
                    .ThenInclude(ct => ct.ViTriChuyenDen)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == dieuChuyenId);
        }

        public async Task<List<DieuChuyen>> TimKiemAsync(DieuChuyenSearchDto dto)
        {
            var query = _context.DieuChuyen
                .Include(x => x.NguoiTao)
                .Include(x => x.PhongBanChuyenDi)
                .Include(x => x.PhongBanChuyenDen)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.SoPhieu))
                query = query.Where(x => x.SoPhieu.Contains(dto.SoPhieu));
            if (dto.TuNgay.HasValue)
                query = query.Where(x => x.NgayTao >= dto.TuNgay.Value);
            if (dto.DenNgay.HasValue)
                query = query.Where(x => x.NgayTao <= dto.DenNgay.Value);
            if (dto.TrangThai.HasValue)
                query = query.Where(x => x.TrangThai == dto.TrangThai.Value);

            return await query.OrderByDescending(x => x.NgayTao).ToListAsync();
        }

        /// <summary>Format DC-{năm}-{stt:D4}, reset theo năm. STT = MAX + 1 trong năm hiện tại (không dùng COUNT).</summary>
        private async Task<string> SinhSoPhieuAsync()
        {
            var year = DateTime.Now.Year;
            var prefix = $"DC-{year}-";

            var soPhieuTrongNam = await _context.DieuChuyen
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