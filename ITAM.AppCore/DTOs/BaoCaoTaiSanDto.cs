using ITAM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.AppCore.DTOs
{
    public class BaoCaoTaiSanDto
    {
        public string MaTaiSan { get; set; } = "";
        public string TenTaiSan { get; set; } = "";
        public string? Serial { get; set; }
        public string? MaHangHoa { get; set; }
        public string TenLoaiTaiSan { get; set; } = "";
        public TrangThaiTaiSan TrangThai { get; set; }   // enum, không map tên hiển thị
        public string TenViTri { get; set; } = "";
        public string TenPhongBan { get; set; } = "";    // dùng để group Excel
        public int? NamSuDung { get; set; }
        public string? SoPhieuNhapChiTiet { get; set; }
        public decimal? GiaNhap;
        public string? GhiChu { get; set; }
    }
    public class BaoCaoTaiSanSearchDto
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public long? PhongBanId { get; set; }
        public long? ViTriTaiSanId { get; set; }
        public long? LoaiTaiSanId { get; set; }
        public long? DMTaiSanId { get; set; }
        public TrangThaiTaiSan? TrangThai { get; set; }
        public string? HangSanXuat { get; set; } // TODO xác nhận: field này vẫn chưa xác nhận có tồn tại trên HangHoa/TaiSanDinhDanh chưa — xem mục cuối
    }
}
