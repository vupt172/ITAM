using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ITAM.AppCore.DTOs
{
    // ---------- Form (ViewModel/View) ----------

    public partial class DieuChuyenVatTuFormDto :ObservableObject
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
        public string TenNguoiDuyet { get; set; } = "Chưa duyệt";
        public DateTime? NgayDuyet { get; set; }
        public string? GhiChu { get; set; }
        public string TrangThai { get; set; } = string.Empty;   // PENDING / APPROVED / REJECTED
    }

    public class DieuChuyenVatTuChiTietFormDto
    {
        public long? Id { get; set; }                            // null = dòng mới chưa lưu
        public long HangHoaId { get; set; }
        public string MaHangHoa { get; set; } = string.Empty;
        public string TenHangHoa { get; set; } = string.Empty;
        public long ViTriChuyenDiId { get; set; }
        public string TenViTriChuyenDi { get; set; } = string.Empty;
        public long ViTriChuyenDenId { get; set; }
        public string TenViTriChuyenDen { get; set; } = string.Empty;
        public int SoLuong { get; set; }                         // TODO xác nhận: kiểu của VatTu.SoLuongTon
        public string? GhiChu { get; set; }
    }

    // ---------- ViewModel -> Service ----------

    public class SaveDieuChuyenVatTuDto
    {
        public long? Id { get; set; }
        public long NguoiTaoId { get; set; }
        public long PhongBanChuyenDiId { get; set; }
        public long PhongBanChuyenDenId { get; set; }
        public string? NguoiGiao { get; set; }
        public string? NguoiNhan { get; set; }
        public string? GhiChu { get; set; }
        public List<SaveDieuChuyenVatTuChiTietDto> ChiTiets { get; set; } = new();
        public List<long> ChiTietIdsXoa { get; set; } = new();
    }

    public class SaveDieuChuyenVatTuChiTietDto
    {
        public long? Id { get; set; }
        public long HangHoaId { get; set; }
        public long ViTriChuyenDiId { get; set; }
        public long ViTriChuyenDenId { get; set; }
        public int SoLuong { get; set; }
        public string? GhiChu { get; set; }
    }

    public class DieuChuyenVatTuSearchDto
    {
        public string? SoPhieu { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public TrangThaiPhieu? TrangThai { get; set; }
    }

    // ---------- Dòng vật tư để chọn ở khu vực "Thêm vật tư" ----------

    public class VatTuKhaDungDto
    {
        public long HangHoaId { get; set; }
        public string MaHangHoa { get; set; } = string.Empty;
        public string TenHangHoa { get; set; } = string.Empty;
        public long ViTriTaiSanId { get; set; }
        public string TenViTri { get; set; } = string.Empty;
        public int SoLuongTon { get; set; }
        public int TonKhaDung { get; set; }          // đã trừ phần các phiếu PENDING khác đang giữ

        // Phân biệt cùng 1 hàng hóa ở nhiều vị trí trong ô chọn
        public string HienThi => $"{MaHangHoa} — {TenViTri}";
    }
}