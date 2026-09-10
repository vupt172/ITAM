using ITAM.Domain.Entities.Identity;
using ITAM.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ITAM.Domain.Entities
{
    /// <summary>
    /// Lô nhập kho (header) — 1 lô có thể gồm nhiều dòng chi tiết,
    /// nhập cả Tài Sản Định Danh (TSCĐ/CCDC) lẫn Vật Tư trong cùng 1 lô.
    /// </summary>
    public class LoNhap
    {
        public long Id { get; set; }
        public string SoLo { get; set; } = string.Empty;
        public DateTime NgayNhap { get; set; }

        public long NhaCungCapId { get; set; }
        public NhaCungCap NhaCungCap { get; set; } = null!;

        // Giả định User.Id kiểu long để đồng bộ CatalogEntity — chỉnh lại nếu thực tế là int.
        public long NguoiLapPhieuId { get; set; }
        public User NguoiLapPhieu { get; set; } = null!;

        public TrangThaiLoNhap TrangThai { get; set; } = TrangThaiLoNhap.PENDING;
        public string? GhiChu { get; set; }

        public ICollection<LoNhapChiTiet> ChiTiets { get; set; } = new List<LoNhapChiTiet>();
    }
}