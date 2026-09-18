using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ITAM.AppCore.DTOs
{
    public class LoNhapFormDto
    {
        public long Id { get; set; }
        public string SoLo { get; set; } = string.Empty;
        public DateTime NgayNhap { get; set; } = DateTime.Today;
        public long NhaCungCapId { get; set; }
        public string? TenNhaCungCap { get; set; }
        public string? NguoiGiao { get; set; } = string.Empty;
        public string? MaHoaDon { get; set; } = string.Empty;
        public long NguoiLapPhieuId { get; set; }
        public string TenNguoiLapPhieu { get; set; } = string.Empty;
        public string TenNguoiDuyet { get; set; } = "Chưa duyệt";   // ⬅ mới
        public string? GhiChu { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "PENDING";
    }

    /// <summary>Dòng chi tiết hiển thị/soạn trên UI. Id = null nghĩa là dòng mới, chưa có trong DB.</summary>
    public class LoNhapChiTietFormDto
    {
        public long? Id { get; set; }
        public string SoLo { get; set; } = string.Empty;
        public long HangHoaId { get; set; }
        public string MaHangHoa { get; set; } = string.Empty;
        public string TenVatPham { get; set; } = string.Empty;
        public int SoLuongNhap { get; set; }
        public decimal DonGia { get; set; }
        public long LoaiTaiSanId { get; set; }
        public string TenLoaiTaiSan { get; set; }   // ⬅ mới
        public decimal ThanhTien => SoLuongNhap * DonGia;
    }

    /// <summary>Gói toàn bộ Phiếu Nhập (header + chi tiết) để lưu 1 lần khi bấm "Lưu".</summary>
    public class SaveLoNhapDto
    {
        public long? Id { get; set; } // null = tạo mới
        public DateTime NgayNhap { get; set; }
        public string? NguoiGiao { get; set; }
        public string? MaHoaDon { get; set; }
        public long NhaCungCapId { get; set; }
        public long NguoiLapPhieuId { get; set; }
        public string? GhiChu { get; set; }
        public List<SaveLoNhapChiTietDto> ChiTiets { get; set; } = new();
        public List<long> ChiTietIdsXoa { get; set; } = new();
    }

    public class SaveLoNhapChiTietDto
    {
        public long? Id { get; set; }
        public long HangHoaId { get; set; }
        public string TenVatPham { get; set; } = string.Empty;   // ⬅ mới
        public int SoLuongNhap { get; set; }
        public decimal DonGia { get; set; }
        public long LoaiTaiSanId { get; set; }
    }

    public class LoNhapSearchDto
    {
        public string? SoLo { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public TrangThaiLoNhap? TrangThai { get; set; }
    }

    // KHONG SU DUNG
    public class CreateLoNhapDto
    {
        public DateTime NgayNhap { get; set; }
        public long NhaCungCapId { get; set; }
        public long NguoiLapPhieuId { get; set; }
        public string? GhiChu { get; set; }
    }
    public class CreateLoNhapChiTietDto
    {
        public long HangHoaId { get; set; }
        public int SoLuongNhap { get; set; }
        public decimal DonGia { get; set; }
        public long LoaiTaiSanId { get; set; }
    }
}