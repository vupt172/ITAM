namespace ITAM.AppCore.DTOs
{
    public class TaiSanDinhDanhListDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Serial { get; set; }
        public string TenHangHoa { get; set; } = string.Empty;
        public string TenLoaiTaiSan { get; set; } = string.Empty;
        public string TrangThaiTaiSan { get; set; } = string.Empty;
        public string TenViTri { get; set; } = string.Empty;
        public int? NamSuDung { get; set; }
        public decimal GiaNhap { get; set; }
        public bool DangChoMuon { get; set; }
    }

    public class UpdateTaiSanDinhDanhDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Serial { get; set; }
    }
}