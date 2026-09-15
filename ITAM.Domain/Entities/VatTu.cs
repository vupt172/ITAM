using ITAM.Domain.Entities;

public class VatTu
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public long HangHoaId { get; set; }
    public HangHoa HangHoa { get; set; } = null!;
    public string? DonViTinh { get; set; }
    public int SoLuongTon { get; set; }
    public long ViTriTaiSanId { get; set; }
    public ViTriTaiSan ViTriTaiSan { get; set; } = null!;
    public string? GhiChu { get; set; }
}