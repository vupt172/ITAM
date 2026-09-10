namespace ITAM.Domain.Entities
{
    /// <summary>
    /// Vật Tư — quản lý theo số lượng, KHÔNG định danh riêng lẻ (khác TaiSanDinhDanh).
    /// Mỗi hàng hóa vật tư tương ứng đúng 1 bản ghi VatTu, số lượng được cộng dồn
    /// mỗi khi duyệt Lô Nhập có dòng chi tiết cùng HangHoaId.
    /// </summary>
    public class VatTu
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;

        public long HangHoaId { get; set; }
        public HangHoa HangHoa { get; set; } = null!;

        /// <summary>Đơn vị tính (cái, hộp, kg...). Đề xuất thêm — bỏ nếu không cần.</summary>
        public string? DonViTinh { get; set; }

        /// <summary>Tổng số lượng tồn kho hiện tại.</summary>
        public int SoLuongTon { get; set; }

        /// <summary>Tổng số lượng đang cho mượn — cache, cập nhật bởi PhieuMuonService khi tạo/trả phiếu mượn.</summary>
        public int SoLuongChoMuon { get; set; }

        /// <summary>Số lượng còn có thể sử dụng/xuất kho. Tính toán, không lưu DB.</summary>
        public int SoLuongKhaDung => SoLuongTon - SoLuongChoMuon;

        public long ViTriTaiSanId { get; set; }
        public ViTriTaiSan ViTriTaiSan { get; set; } = null!;
    }
}
