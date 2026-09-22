namespace ITAM.Shared.Constants
{
    /// <summary>
    /// Code cố định của các Phòng Ban hệ thống — mỗi phòng ban này có đúng 1 ViTriTaiSan tương ứng
    /// (xem ITAM.Shared.Constants.ViTriTaiSanCodes, quan hệ 1-1). Seed sẵn qua DbSeeder.
    /// </summary>
    public static class PhongBanCodes
    {
        /// <summary>Phòng Ban "Kho Lưu Trữ" — có 1 ViTriTaiSan duy nhất: ViTriTaiSanCodes.KHO_LUU_TRU.</summary>
        public const string KHO_LUU_TRU = "KHO_LUU_TRU";

        /// <summary>Phòng Ban "Kho Thất Lạc" — có 1 ViTriTaiSan duy nhất: ViTriTaiSanCodes.KHO_THAT_LAC. Không chọn được làm Phòng Ban Chuyển Đến trong màn hình Điều Chuyển thường.</summary>
        public const string KHO_THAT_LAC = "KHO_THAT_LAC";

        /// <summary>Phòng Ban "Kho Chờ Thanh Lý" — có 1 ViTriTaiSan duy nhất: ViTriTaiSanCodes.KHO_CHO_THANH_LY. Chọn được làm Phòng Ban Chuyển Đến trong màn hình Điều Chuyển thường.</summary>
        public const string KHO_CHO_THANH_LY = "KHO_CHO_THANH_LY";

        /// <summary>Phòng Ban "Kho Đã Thanh Lý" — có 1 ViTriTaiSan duy nhất: ViTriTaiSanCodes.KHO_THANH_LY. Không chọn được làm Phòng Ban Chuyển Đến trong màn hình Điều Chuyển thường.</summary>
        public const string KHO_THANH_LY = "KHO_THANH_LY";
    }
}