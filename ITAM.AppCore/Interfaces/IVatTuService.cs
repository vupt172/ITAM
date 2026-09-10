using System.Collections.Generic;
using System.Threading.Tasks;
using ITAM.Domain.Entities;

namespace ITAM.AppCore.Interfaces
{
    /// <summary>
    /// Truy vấn Vật Tư — nền tảng phục vụ các chức năng sau này như chuyển phòng ban,
    /// thanh lý vật tư... Hiện tại chỉ có các method query cơ bản.
    /// Lưu ý: VatTu không có trạng thái dạng enum như TaiSanDinhDanh (chỉ có SoLuongTon/SoLuongChoMuon),
    /// nên không có GetByTrangThaiAsync tương ứng.
    /// </summary>
    public interface IVatTuService
    {
        Task<VatTu?> GetByIdAsync(long id);
        Task<List<VatTu>> GetAllAsync();

        /// <summary>Danh sách vật tư đang ở 1 vị trí (kho/phòng ban) cụ thể.</summary>
        Task<List<VatTu>> GetByViTriAsync(long viTriTaiSanId);
        /// <summary>Sửa Name/Code/DonViTinh — KHÔNG cho đổi ViTriTaiSanId ở đây, phải qua chức năng chuyển vật tư sau này.</summary>
        Task UpdateAsync(UpdateVatTuDto dto);
    }
}