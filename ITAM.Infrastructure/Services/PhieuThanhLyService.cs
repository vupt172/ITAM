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
    public class PhieuThanhLyService : IPhieuThanhLyService
    {
        private readonly AppDbContext _context;

        public PhieuThanhLyService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<long> LuuPhieuAsync(SavePhieuThanhLyDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrWhiteSpace(dto.LyDo))
                    throw new InvalidBusinessRuleException("Chưa nhập lý do thanh lý.");

                if (dto.ChiTiets.Any(x => x.GiaThanhLy < 0))
                    throw new InvalidBusinessRuleException("Giá thanh lý không được âm.");

                PhieuThanhLy phieu;

                if (dto.Id is long id && id > 0)
                {
                    phieu = await _context.PhieuThanhLy
                        .Include(x => x.ChiTiets)
                        .FirstOrDefaultAsync(x => x.Id == id)
                        ?? throw new InvalidBusinessRuleException("Không tìm thấy Phiếu Thanh Lý.");

                    if (phieu.TrangThai != TrangThaiPhieu.PENDING)
                        throw new InvalidBusinessRuleException("Phiếu đã được duyệt hoặc đã từ chối, không thể sửa.");
                }
                else
                {
                    // Chỉ Phòng Ban Kho Chờ Thanh Lý mới được lập phiếu. Re-check ở service vì user có thể đổi
                    // Phòng Ban mặc định (ChangePhongBanWindow) khi màn hình đang mở.
                    var phongBan = await _context.PhongBan.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == dto.PhongBanId)
                        ?? throw new InvalidBusinessRuleException("Không tìm thấy Phòng Ban của phiếu.");

                    if (phongBan.Code != PhongBanCodes.KHO_CHO_THANH_LY)
                        throw new InvalidBusinessRuleException(
                            "Chỉ người dùng có Phòng Ban mặc định là Kho Chờ Thanh Lý mới được lập phiếu thanh lý.");

                    phieu = new PhieuThanhLy
                    {
                        SoPhieu = await SinhSoPhieuAsync(),
                        NgayTao = DateTime.Now,
                        NguoiTaoId = dto.NguoiTaoId,
                        PhongBanId = phongBan.Id,
                        TrangThai = TrangThaiPhieu.PENDING
                    };
                    _context.PhieuThanhLy.Add(phieu);
                }

                phieu.LyDo = dto.LyDo.Trim();

                foreach (var xoaId in dto.ChiTietIdsXoa)
                {
                    var ct = phieu.ChiTiets.FirstOrDefault(x => x.Id == xoaId);
                    if (ct != null) _context.PhieuThanhLyChiTiet.Remove(ct);
                }

                foreach (var ctDto in dto.ChiTiets)
                {
                    if (ctDto.Id is long ctId && ctId > 0)
                    {
                        var ct = phieu.ChiTiets.FirstOrDefault(x => x.Id == ctId)
                            ?? throw new InvalidBusinessRuleException("Không tìm thấy dòng chi tiết cần cập nhật.");
                        // Chỉ cho sửa Giá thanh lý + Ghi chú — TaiSanDinhDanhId/ViTriTruocId là snapshot lúc thêm dòng.
                        ct.GiaThanhLy = ctDto.GiaThanhLy;
                        ct.GhiChu = ctDto.GhiChu;
                        continue;
                    }

                    if (phieu.ChiTiets.Any(x => x.TaiSanDinhDanhId == ctDto.TaiSanDinhDanhId
                                                && !dto.ChiTietIdsXoa.Contains(x.Id)))
                        throw new InvalidBusinessRuleException("Tài sản đã có trong phiếu.");

                    var taiSan = await KiemTraTaiSanHopLeAsync(ctDto.TaiSanDinhDanhId, phieu.Id);

                    // Add qua navigation collection để EF tự fixup FK lúc SaveChanges (phieu.Id = 0 khi tạo mới).
                    phieu.ChiTiets.Add(new PhieuThanhLyChiTiet
                    {
                        TaiSanDinhDanhId = taiSan.Id,
                        ViTriTruocId = taiSan.ViTriTaiSanId,   // snapshot lấy từ DB, không tin DTO
                        GiaThanhLy = ctDto.GiaThanhLy,
                        GhiChu = ctDto.GhiChu
                    });
                }

                if (phieu.ChiTiets.Count(x => !dto.ChiTietIdsXoa.Contains(x.Id)) == 0)
                    throw new InvalidBusinessRuleException("Phiếu thanh lý phải có ít nhất 1 tài sản.");

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

        /// <summary>Kiểm tra 1 tài sản có được thêm vào phiếu thanh lý (lúc Lưu nháp). Trả về tài sản để lấy snapshot.</summary>
        private async Task<TaiSanDinhDanh> KiemTraTaiSanHopLeAsync(long taiSanId, long phieuIdHienTai)
        {
            var taiSan = await _context.TaiSanDinhDanh.AsNoTracking()
                .Include(x => x.ViTriTaiSan)
                .FirstOrDefaultAsync(x => x.Id == taiSanId)
                ?? throw new InvalidBusinessRuleException("Không tìm thấy tài sản.");

            if (taiSan.TrangThaiTaiSan == TrangThaiTaiSan.DISPOSED)
                throw new InvalidBusinessRuleException($"Tài sản '{taiSan.Code}' đã được thanh lý.");

            if (taiSan.TrangThaiTaiSan != TrangThaiTaiSan.DISPOSAL_PENDING)
                throw new InvalidBusinessRuleException(
                    $"Tài sản '{taiSan.Code}' chưa ở trạng thái Chờ Thanh Lý.");

            if (taiSan.ViTriTaiSan.Code != ViTriTaiSanCodes.KHO_CHO_THANH_LY)
                throw new InvalidBusinessRuleException(
                    $"Tài sản '{taiSan.Code}' không nằm trong Kho Chờ Thanh Lý.");

            var dangOPhieuThanhLyKhac = await _context.PhieuThanhLyChiTiet.AsNoTracking()
                .AnyAsync(x => x.TaiSanDinhDanhId == taiSanId
                    && x.PhieuThanhLyId != phieuIdHienTai
                    && x.PhieuThanhLy.TrangThai == TrangThaiPhieu.PENDING);
            if (dangOPhieuThanhLyKhac)
                throw new InvalidBusinessRuleException(
                    $"Tài sản '{taiSan.Code}' đang có trong 1 Phiếu Thanh Lý khác chưa được duyệt.");

            var dangODieuChuyen = await _context.DieuChuyenChiTiet.AsNoTracking()
                .AnyAsync(x => x.TaiSanDinhDanhId == taiSanId
                    && x.DieuChuyen.TrangThai == TrangThaiPhieu.PENDING);
            if (dangODieuChuyen)
                throw new InvalidBusinessRuleException(
                    $"Tài sản '{taiSan.Code}' đang có trong 1 Phiếu Điều Chuyển chưa được duyệt.");

            return taiSan;
        }

        public async Task DuyetPhieuAsync(long phieuThanhLyId, long nguoiDuyetId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var phieu = await _context.PhieuThanhLy
                    .Include(x => x.ChiTiets)
                        .ThenInclude(ct => ct.TaiSanDinhDanh)
                            .ThenInclude(ts => ts.ViTriTaiSan)
                    .FirstOrDefaultAsync(x => x.Id == phieuThanhLyId)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Phiếu Thanh Lý.");

                if (phieu.TrangThai != TrangThaiPhieu.PENDING)
                    throw new InvalidBusinessRuleException("Phiếu đã được duyệt hoặc đã từ chối, không thể duyệt lại.");

                if (!phieu.ChiTiets.Any())
                    throw new InvalidBusinessRuleException("Phiếu thanh lý chưa có dòng chi tiết nào.");

                var khoThanhLyId = await _context.ViTriTaiSan.AsNoTracking()
                    .Where(x => x.Code == ViTriTaiSanCodes.KHO_THANH_LY)
                    .Select(x => (long?)x.Id)
                    .FirstOrDefaultAsync()
                    ?? throw new InvalidBusinessRuleException("Chưa có Vị Trí Kho Thanh Lý trong hệ thống.");

                var thoiDiemDuyet = DateTime.Now;

                foreach (var ct in phieu.ChiTiets)
                {
                    var taiSan = ct.TaiSanDinhDanh;

                    // Re-check lúc duyệt: trạng thái/vị trí tài sản có thể đã đổi kể từ lúc tạo phiếu.
                    if (taiSan.TrangThaiTaiSan != TrangThaiTaiSan.DISPOSAL_PENDING)
                        throw new InvalidBusinessRuleException(
                            $"Tài sản '{taiSan.Code}' không còn ở trạng thái Chờ Thanh Lý, hãy từ chối phiếu này và lập lại.");

                    if (taiSan.ViTriTaiSan.Code != ViTriTaiSanCodes.KHO_CHO_THANH_LY)
                        throw new InvalidBusinessRuleException(
                            $"Tài sản '{taiSan.Code}' đã chuyển khỏi Kho Chờ Thanh Lý, hãy từ chối phiếu này và lập lại.");

                    var viTriDiId = taiSan.ViTriTaiSanId;   // vị trí thực tế hiện tại, không dùng snapshot ct.ViTriTruocId

                    taiSan.TrangThaiTaiSan = TrangThaiTaiSan.DISPOSED;
                    taiSan.ViTriTaiSanId = khoThanhLyId;

                    _context.LichSuDieuChuyenTaiSan.Add(new LichSuDieuChuyenTaiSan
                    {
                        TaiSanDinhDanhId = taiSan.Id,
                        ViTriChuyenDiId = viTriDiId,
                        ViTriChuyenDenId = khoThanhLyId,
                        NgayDuyet = thoiDiemDuyet,
                        NguoiDuyetId = nguoiDuyetId,
                        PhieuThanhLyChiTietId = ct.Id,
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

        public async Task TuChoiPhieuAsync(long phieuThanhLyId, long nguoiDuyetId)
        {
            try
            {
                var phieu = await _context.PhieuThanhLy.FirstOrDefaultAsync(x => x.Id == phieuThanhLyId)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Phiếu Thanh Lý.");

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

        public async Task DeleteAsync(long phieuThanhLyId)
        {
            try
            {
                var phieu = await _context.PhieuThanhLy
                    .Include(x => x.ChiTiets)
                    .FirstOrDefaultAsync(x => x.Id == phieuThanhLyId)
                    ?? throw new InvalidBusinessRuleException("Không tìm thấy Phiếu Thanh Lý.");

                if (phieu.TrangThai != TrangThaiPhieu.PENDING)
                    throw new InvalidBusinessRuleException("Chỉ được xóa phiếu đang ở trạng thái Chờ duyệt.");

                _context.PhieuThanhLy.Remove(phieu);   // Cascade xóa ChiTiets theo configuration.
                await _context.SaveChangesAsync();
            }
            catch
            {
                _context.ChangeTracker.Clear();
                throw;
            }
        }

        public async Task<PhieuThanhLy?> GetByIdAsync(long phieuThanhLyId)
        {
            return await _context.PhieuThanhLy
                .Include(x => x.NguoiTao)
                .Include(x => x.NguoiDuyet)
                .Include(x => x.PhongBan)
                .Include(x => x.ChiTiets)
                    .ThenInclude(ct => ct.TaiSanDinhDanh)
                .Include(x => x.ChiTiets)
                    .ThenInclude(ct => ct.ViTriTruoc)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == phieuThanhLyId);
        }

        public async Task<List<PhieuThanhLy>> TimKiemAsync(PhieuThanhLySearchDto dto)
        {
            var query = _context.PhieuThanhLy
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

        /// <summary>Format THL-{năm}-{stt:D4}, reset theo năm. STT = MAX + 1 trong năm hiện tại.</summary>
        private async Task<string> SinhSoPhieuAsync()
        {
            var prefix = $"THL-{DateTime.Now.Year}-";

            var soPhieuTrongNam = await _context.PhieuThanhLy
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