namespace ITAM.Shared.Constants
{
    /// <summary>
    /// Code cố định của các Vị Trí Tài Sản hệ thống (được DbSeeder seed sẵn, cố định 1 bản ghi duy nhất
    /// cho mỗi loại). Để dạng hằng số thay vì appsettings.json vì đây là dữ liệu nghiệp vụ gắn chặt với
    /// DbSeeder và state machine của TrangThaiTaiSan — thay đổi giá trị Code bắt buộc phải đồng bộ với
    /// seed data và logic service, nên việc "cấu hình được" qua appsettings không có lợi ích thực tế mà
    /// lại mất kiểm tra lúc biên dịch (gõ sai chuỗi trong appsettings sẽ chỉ lỗi lúc chạy).
    /// </summary>
    public static class ViTriTaiSanCodes
    {
        /// <summary>Kho Lưu Trữ — cũng là kho nhận tài sản mới sinh ra khi Duyệt Lô Nhập.</summary>
        public const string KHO_LUU_TRU = "VT_KHO_LUU_TRU";

        /// <summary>Kho Thất Lạc — ứng với TrangThaiTaiSan.LOST. Không chọn được làm đích trong màn hình Điều Chuyển thường.</summary>
        public const string KHO_THAT_LAC = "VT_KHO_THAT_LAC";

        /// <summary>Kho Chờ Thanh Lý — ứng với TrangThaiTaiSan.DISPOSAL_PENDING. Chọn được làm đích trong màn hình Điều Chuyển thường.</summary>
        public const string KHO_CHO_THANH_LY = "VT_KHO_CHO_THANH_LY";

        /// <summary>Kho Đã Thanh Lý — ứng với TrangThaiTaiSan.DISPOSED. Không chọn được làm đích trong màn hình Điều Chuyển thường (có chức năng Thanh Lý riêng).</summary>
        public const string KHO_THANH_LY = "VT_KHO_THANH_LY";
    }
}