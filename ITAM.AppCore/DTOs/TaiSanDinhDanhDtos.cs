namespace ITAM.AppCore.DTOs
{
    public class TaiSanDinhDanhListDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Serial { get; set; }
        public string MaHangHoa { get; set; } = string.Empty;
        public string TenLoaiTaiSan { get; set; } = string.Empty;
        public string TrangThaiTaiSan { get; set; } = string.Empty;
        public string TenViTri { get; set; } = string.Empty;
        public int? NamSuDung { get; set; }
        public decimal GiaNhap { get; set; }
        public string SoLoNhapChiTiet { get; set; } = string.Empty; // lấy từ LoNhapChiTiet.SoLo
        public string? GhiChu { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty; // ⬅ mới — lấy từ HangHoa.DMTaiSan.Name

    }

    public class UpdateTaiSanDinhDanhDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Serial { get; set; }
        public int? NamSuDung { get; set; }
        public string? GhiChu { get; set; }
    }
}