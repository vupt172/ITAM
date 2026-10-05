using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities;
using ITAM.Domain.Enums;
using ITAM.Domain.Exceptions;
using ITAM.Infrastructure.Data;
using ITAM.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Infrastructure.Services
{
    public class PhieuThatLacService : IPhieuThatLacService
    {
        private readonly AppDbContext _context;

        public PhieuThatLacService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<long> LuuPhieuAsync(SavePhieuThatLacDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrWhiteSpace(dto.LyDo))
                    throw new InvalidBusinessRuleException("Chưa nhập lý do/mô tả sự việc.");

                PhieuThatLac phieu;

                if (dto.Id is long id && id > 0)
                {
                    phieu = await _context.PhieuThatLac
                        .Include(x => x.ChiTiets)
                        .FirstOrDefaultAsync(x => x.Id == id)
                        ?? throw new InvalidBusinessRuleException("Không tìm thấy Phiếu Thất Lạc.");

                    if (phieu.TrangThai != TrangThaiPhieu.PENDING)
                        throw new InvalidBusinessRuleException("Phiếu đã được duyệt hoặc đã từ chối, không thể sửa.");
                }
                else
                {
                    phieu = new PhieuThatLac
                    {
                        SoPhieu = await SinhSoPhieuAsync(),
                        NgayTao = DateTime.Now,
                        NguoiTaoId = dto.NguoiTaoId,
                        // Phòng Ban cố định = Phòng Ban mặc định của người tạo (ViewModel gán sẵn), không cho sửa sau khi tạo.
                        PhongBanId = dto.PhongBanId,
                        TrangThai = TrangThaiPhieu.PENDING
                    };
                    _context.PhieuThatLac.Add(phieu);
                }

                phieu.NgayPhatHien = dto.NgayPhatHien;
                phieu.NguoiPhatHien = dto.NguoiPhatHien;
                phieu.LyDo = dto.LyDo.Trim();

                foreach (var xoaId in dto.ChiTietIdsXoa)
                {
                    var ct = phieu.ChiTiets.FirstOrDefault(x => x.Id == xoaId);
                    if (ct != null) _context.PhieuThatLacChiTiet.Remove(ct);
                }

                foreach (var ctDto in dto.ChiTiets)
                {
                    if (ctDto.Id is long ctId && ctId > 0)
                    {
                        var ct = phieu.ChiTiets.FirstOrDefault(x => x.Id == ctId)
                            ?? throw new InvalidBusinessRuleException("Không tìm thấy dòng chi tiết cần cập nhật.");
                        // Chỉ cho sửa Ghi chú — TaiSanDinhDanhId/ViTriTruocId/TrangThaiTruoc là snapshot lúc thêm dòng.
                        ct.GhiChu = ctDto.GhiChu;
                        continue;
                    }

                    if (phieu.ChiTiets.Any(x => x.TaiSanDinhDanhId == ctDto.TaiSanDinhDanhId
                                                && !dto.ChiTietIdsXoa.Contains(x.Id)))
                        throw new InvalidBusinessRuleException("Tài sản đã có trong phiếu.");

                    var taiSan = await KiemTraTaiSanHopLeAsync(ctDto.TaiSanDinhDanhId, phieu.Id, phieu.PhongBanId);

                    // Add qua navigation collection để EF tự fixup FK lúc SaveChanges (phieu.Id = 0 khi tạo mới).
                    phieu.ChiTiets.Add(new PhieuThatLacChiTiet
                    {
                        TaiSanDinhDanhId = taiSan.Id,
                        ViTriTruocId = taiSan.ViTriTaiSanId,       // snapshot lấy từ DB, không tin DTO
                        TrangThaiTruoc = taiSan.TrangThaiTaiSan,   // snapshot
                        GhiChu = ctDto.GhiChu
                    });
                }

                if (phieu.ChiTiets.Count(x => !dto.ChiTietIdsXoa.Contains(x.Id)) == 0)
                    throw new InvalidBusinessRuleException("Phiếu thất lạc phải có ít nhất 1 tài sản.");

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

        /// <summary>Kiểm tra 1 tài sản có được thêm vào phiếu thất lạc (lúc Lưu nháp). Trả về tài sản để lấy snapshot.</summary>
        private async Task<TaiSanDinhDanh> KiemTraTaiSanHopLeAsync(long taiSanId, long phieuIdHienTai, long phongBanId)
        {
            var taiSan = await _context.TaiSanDinhDanh.AsNoTracking()
                .Include(x => x.ViTriTaiSan)
                .FirstOrDefaultAsync(x => x.Id == taiSanId)
                ?? throw new InvalidBusinessRuleException("Không tìm thấy tài sản.");

            if (taiSan.TrangThaiTaiSan == TrangThaiTaiSan.MAINTENANCE)
                throw new InvalidBusinessRuleException(
                    $"Tài sản '{taiSan.Code}' đang sửa chữa, cần hoàn thành sửa chữa trước khi khai báo thất lạc.");

            if (taiSan.TrangThaiTaiSan == TrangThaiTaiSan.LOST)
                throw new InvalidBusinessRuleException($"Tài sản '{taiSan.Code}' đã ở trạng thái Thất Lạc.");

            if (taiSan.TrangThaiTaiSan is not (TrangThaiTaiSan.IN_STOCK or TrangThaiTaiSan.ASSIGNED))
                throw new InvalidBusinessRuleException(
                    $"Tài sản '{taiSan.Code}' không ở trạng thái hợp lệ để khai báo thất lạc.");

            if (taiSan.ViTriTaiSan.PhongBanId != phongBanId)
                throw new InvalidBusinessRuleException($"Tài sản '{taiSan.Code}' không thuộc Phòng Ban của phiếu.");

            var dangOPhieuThatLacKhac = await _context.PhieuThatLacChiTiet.AsNoTracking()
                .AnyAsync(x => x.TaiSanDinhDanhId == taiSanId
                    && x.PhieuThatLacId != phieuIdHienTai
                    && x.PhieuThatLac.TrangThai == TrangThaiPhieu.PENDING);
            if (dangOPhieuThatLacKhac)
                throw new InvalidBusinessRuleException(
                    $"Tài sản '{taiSan.Code}' đang có trong 1 Phiếu Thất Lạc khác chưa được duyệt.");

            var dangODieuChuyen = await _context.DieuChuyenChiTiet.AsNoTracking()
                .AnyAsync(x => x.TaiSanDinhDanhId == taiSanId
                    && x.DieuChuyen.TrangThai == TrangThaiPhieu.PENDING);
            if (dangODieuChuyen)
                throw new InvalidBusinessRuleException(
                    $"Tài sản '{taiSan.Code}' đang có trong 1 Phiếu Điều Chuyển chưa được duyệt.");

            return taiSan;
        }

        public async Task DuyetPhieuAsync(long phieuThatLacId, long nguoiDuyetId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var phieu = await _context.PhieuThatLac
                    .Include(x => x.ChiTiets)
                        .ThenInclude(ct => ct.TaiSanDinhDanh)
                            .ThenInclude(ts => ts.ViTriTaiSan)
                    .FirstOrDefaultAsync(x => x.Id == phieuThatLacId)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Phiếu Thất Lạc.");

                if (phieu.TrangThai != TrangThaiPhieu.PENDING)
                    throw new InvalidBusinessRuleException("Phiếu đã được duyệt hoặc đã từ chối, không thể duyệt lại.");

                if (!phieu.ChiTiets.Any())
                    throw new InvalidBusinessRuleException("Phiếu thất lạc chưa có dòng chi tiết nào.");

                var khoThatLacId = await _context.ViTriTaiSan.AsNoTracking()
                    .Where(x => x.Code == ViTriTaiSanCodes.KHO_THAT_LAC)
                    .Select(x => (long?)x.Id)
                    .FirstOrDefaultAsync()
                    ?? throw new InvalidBusinessRuleException("Chưa có Vị Trí Kho Thất Lạc trong hệ thống.");

                var thoiDiemDuyet = DateTime.Now;

                foreach (var ct in phieu.ChiTiets)
                {
                    var taiSan = ct.TaiSanDinhDanh;

                    // Re-check lúc duyệt: trạng thái/vị trí tài sản có thể đã đổi kể từ lúc tạo phiếu.
                    if (taiSan.TrangThaiTaiSan == TrangThaiTaiSan.MAINTENANCE)
                        throw new InvalidBusinessRuleException($"Tài sản '{taiSan.Code}' đang sửa chữa, không thể duyệt thất lạc.");

                    if (taiSan.TrangThaiTaiSan is not (TrangThaiTaiSan.IN_STOCK or TrangThaiTaiSan.ASSIGNED))
                        throw new InvalidBusinessRuleException(
                            $"Tài sản '{taiSan.Code}' không còn ở trạng thái hợp lệ để duyệt thất lạc.");

                    if (taiSan.ViTriTaiSan.PhongBanId != phieu.PhongBanId)
                        throw new InvalidBusinessRuleException(
                            $"Tài sản '{taiSan.Code}' đã chuyển khỏi Phòng Ban của phiếu, hãy từ chối phiếu này và lập lại.");

                    var viTriDiId = taiSan.ViTriTaiSanId;   // vị trí thực tế hiện tại, không dùng snapshot ct.ViTriTruocId

                    taiSan.TrangThaiTaiSan = TrangThaiTaiSan.LOST;
                    taiSan.ViTriTaiSanId = khoThatLacId;

                    _context.LichSuDieuChuyenTaiSan.Add(new LichSuDieuChuyenTaiSan
                    {
                        TaiSanDinhDanhId = taiSan.Id,
                        ViTriChuyenDiId = viTriDiId,
                        ViTriChuyenDenId = khoThatLacId,
                        NgayDuyet = thoiDiemDuyet,
                        NguoiDuyetId = nguoiDuyetId,
                        PhieuThatLacChiTietId = ct.Id,
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

        public async Task TuChoiPhieuAsync(long phieuThatLacId, long nguoiDuyetId)
        {
            try
            {
                var phieu = await _context.PhieuThatLac.FirstOrDefaultAsync(x => x.Id == phieuThatLacId)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Phiếu Thất Lạc.");

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

        public async Task DeleteAsync(long phieuThatLacId)
        {
            try
            {
                var phieu = await _context.PhieuThatLac
                    .Include(x => x.ChiTiets)
                    .FirstOrDefaultAsync(x => x.Id == phieuThatLacId)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Phiếu Thất Lạc.");

                if (phieu.TrangThai != TrangThaiPhieu.PENDING)
                    throw new InvalidBusinessRuleException("Chỉ được xóa phiếu đang ở trạng thái Chờ duyệt.");

                _context.PhieuThatLac.Remove(phieu);   // Cascade xóa ChiTiets theo configuration.
                await _context.SaveChangesAsync();
            }
            catch
            {
                _context.ChangeTracker.Clear();
                throw;
            }
        }

        public async Task<PhieuThatLac?> GetByIdAsync(long phieuThatLacId)
        {
            return await _context.PhieuThatLac
                .Include(x => x.NguoiTao)
                .Include(x => x.NguoiDuyet)
                .Include(x => x.PhongBan)
                .Include(x => x.ChiTiets)
                    .ThenInclude(ct => ct.TaiSanDinhDanh)
                .Include(x => x.ChiTiets)
                    .ThenInclude(ct => ct.ViTriTruoc)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == phieuThatLacId);
        }

        public async Task<List<PhieuThatLac>> TimKiemAsync(PhieuThatLacSearchDto dto)
        {
            var query = _context.PhieuThatLac
                .Include(x => x.NguoiTao)
                .Include(x => x.PhongBan)
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

        /// <summary>Format TL-{năm}-{stt:D4}, reset theo năm. STT = MAX + 1 trong năm hiện tại.</summary>
        private async Task<string> SinhSoPhieuAsync()
        {
            var prefix = $"TL-{DateTime.Now.Year}-";

            var soPhieuTrongNam = await _context.PhieuThatLac
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