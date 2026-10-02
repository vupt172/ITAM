namespace ITAM.Shared.Constants;

public static class LoaiTaiSanCodes
{
    public const string TaiSanCoDinh = "TSCD";   // TODO xác nhận: Code do mình tự đặt
    public const string CongCuDungCu = "CCDC";   // TODO xác nhận
    public const string VatTu = "VATTU";  // đã có sẵn trong logic Duyệt Lô Nhập

    /// Ngưỡng phân loại theo giá trị (VNĐ)
    public const decimal NguongTaiSanCoDinh = 10_000_000m;
}