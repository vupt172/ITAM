namespace ITAM.Domain.Enums
{
    /// <summary>Trạng thái Phiếu Điều Chuyển tài sản.</summary>
    public enum TrangThaiDieuChuyen
    {
        /// <summary>Mới tạo / đang chờ duyệt. Có thể sửa hoặc xóa.</summary>
        PENDING,

        /// <summary>Đã duyệt — đã cập nhật Vị Trí + Trạng Thái tài sản và ghi Lịch Sử Điều Chuyển. Không thể sửa/xóa.</summary>
        APPROVED,

        /// <summary>Đã từ chối. Không thay đổi gì trên tài sản. Không thể sửa/xóa.</summary>
        REJECTED
    }
}