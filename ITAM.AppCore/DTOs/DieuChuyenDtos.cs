using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ITAM.AppCore.DTOs
{
    /// <summary>Dữ liệu hiển thị/binding cho form Phiếu Điều Chuyển (đọc).</summary>
    public partial class DieuChuyenFormDto: ObservableObject
    {
        public long Id { get; set; }
        public string SoPhieu { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
        public long NguoiTaoId { get; set; }
        public string TenNguoiTao { get; set; } = string.Empty;


        [ObservableProperty] private long phongBanChuyenDiId;
        public string TenPhongBanChuyenDi { get; set; } = string.Empty;
        public long PhongBanChuyenDenId { get; set; }
        public string TenPhongBanChuyenDen { get; set; } = string.Empty;

        public string? NguoiGiao { get; set; }
        public string? NguoiNhan { get; set; }

        public string TrangThai { get; set; } = "PENDING";
        public long? NguoiDuyetId { get; set; }
        public string TenNguoiDuyet { get; set; } = "Chưa duyệt";
        public DateTime? NgayDuyet { get; set; }
        public string? GhiChu { get; set; }
    }

    /// <summary>Dữ liệu hiển thị/binding cho 1 dòng chi tiết trong lưới ChiTiets (đọc).</summary>
    public class DieuChuyenChiTietFormDto
    {
        /// <summary>Null = dòng mới thêm, chưa lưu DB.</summary>
        public long? Id { get; set; }

        public long TaiSanDinhDanhId { get; set; }
        public string MaTaiSan { get; set; } = string.Empty;
        public string TenTaiSan { get; set; } = string.Empty;

        public long ViTriChuyenDiId { get; set; }
        public string TenViTriChuyenDi { get; set; } = string.Empty;

        public long ViTriChuyenDenId { get; set; }
        public string TenViTriChuyenDen { get; set; } = string.Empty;

        public string? GhiChu { get; set; }
    }

    /// <summary>Payload gửi xuống service khi Lưu (tạo mới hoặc cập nhật) 1 phiếu điều chuyển ở trạng thái PENDING.</summary>
    public class SaveDieuChuyenDto
    {
        /// <summary>Null hoặc 0 = tạo phiếu mới.</summary>
        public long? Id { get; set; }

        public long NguoiTaoId { get; set; }

        /// <summary>Luôn = Phòng Ban mặc định của NguoiTaoId — do ViewModel tự gán, không cho người dùng chọn tay.</summary>
        public long PhongBanChuyenDiId { get; set; }

        /// <summary>Người tạo phiếu chọn — áp dụng chung cho toàn bộ dòng chi tiết trong phiếu.</summary>
        public long PhongBanChuyenDenId { get; set; }

        public string? NguoiGiao { get; set; }
        public string? NguoiNhan { get; set; }
        public string? GhiChu { get; set; }

        public List<SaveDieuChuyenChiTietDto> ChiTiets { get; set; } = new();

        /// <summary>Id các dòng chi tiết đã có trong DB (Id &gt; 0) cần xóa khỏi phiếu.</summary>
        public List<long> ChiTietIdsXoa { get; set; } = new();
    }

    public class SaveDieuChuyenChiTietDto
    {
        /// <summary>Null hoặc &lt;= 0 = dòng mới thêm.</summary>
        public long? Id { get; set; }

        public long TaiSanDinhDanhId { get; set; }
        public long ViTriChuyenDiId { get; set; }
        public long ViTriChuyenDenId { get; set; }
        public string? GhiChu { get; set; }
    }

    /// <summary>Điều kiện tìm kiếm phiếu điều chuyển (dùng ở DieuChuyenSearchWindow).</summary>
    public class DieuChuyenSearchDto
    {
        public string? SoPhieu { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public TrangThaiDieuChuyen? TrangThai { get; set; }
    }

    /// <summary>1 dòng lịch sử điều chuyển hiển thị (đọc) — dùng cho màn hình xem Lịch Sử Điều Chuyển của 1 tài sản.</summary>
    public class LichSuDieuChuyenTaiSanDto
    {
        public long Id { get; set; }
        public long TaiSanDinhDanhId { get; set; }
        public string MaTaiSan { get; set; } = string.Empty;
        public string TenTaiSan { get; set; } = string.Empty;
        public string? TenViTriChuyenDi { get; set; } // null = "— (mới nhập kho)"
        public string TenViTriChuyenDen { get; set; } = string.Empty;
        public DateTime NgayDuyet { get; set; }
        public string TenNguoiDuyet { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
    }

    /// <summary>Điều kiện tìm kiếm/tổng hợp Lịch Sử Điều Chuyển — phục vụ báo cáo (ILichSuDieuChuyenService.TimKiemAsync).</summary>
    public class LichSuDieuChuyenSearchDto
    {
        public long? TaiSanDinhDanhId { get; set; }

        /// <summary>Lọc theo Vị Trí liên quan — khớp cả ViTriChuyenDi lẫn ViTriChuyenDen (VD: "mọi thứ đi/đến Phòng Kế Toán").</summary>
        public long? ViTriTaiSanId { get; set; }

        public long? NguoiDuyetId { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }
}