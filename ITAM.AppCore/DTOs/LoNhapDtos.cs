using ITAM.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ITAM.AppCore.DTOs
{
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

        /// <summary>
        /// Người dùng chọn từ combobox (đã filter theo DonGia ở UI).
        /// Bắt buộc nếu DMTaiSan.IsTrackedById = true.
        /// </summary>
        public long? LoaiTaiSanId { get; set; }
    }

    public class LoNhapFormDto
    {
        public long Id { get; set; }
        public string SoLo { get; set; } = string.Empty;
        public DateTime NgayNhap { get; set; } = DateTime.Today;

        public long NhaCungCapId { get; set; }
        public string? TenNhaCungCap { get; set; }

        public long NguoiLapPhieuId { get; set; }
        public string TenNguoiLapPhieu { get; set; } = string.Empty;

        public string? GhiChu { get; set; }
        public string TrangThai { get; set; } = "PENDING";
    }
    /// <summary>Tiêu chí tìm kiếm Lô Nhập — dùng cho ILoNhapService.TimKiemAsync.</summary>
    public class LoNhapSearchDto
    {
        public string? SoLo { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public TrangThaiLoNhap? TrangThai { get; set; }
    }
    public class LoNhapChiTietFormDto
    {
        public long Id { get; set; }
        public string SoLo { get; set; } = string.Empty;
        public long HangHoaId { get; set; }
        public string TenVatPham { get; set; } = string.Empty;
        public int SoLuongNhap { get; set; }
        public decimal DonGia { get; set; }
        public long? LoaiTaiSanId { get; set; }
        public decimal ThanhTien => SoLuongNhap * DonGia;
    }
}
