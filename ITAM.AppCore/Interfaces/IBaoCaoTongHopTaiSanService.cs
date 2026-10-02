using ITAM.AppCore.DTOs;

namespace ITAM.AppCore.Interfaces;

public interface IBaoCaoTongHopTaiSanService
{
    Task<TongHopTaiSanLookupDto> GetLookupAsync();

    Task<List<TongHopTaiSanRowDto>> GetAsync(TongHopTaiSanFilterDto filter);

    /// <param name="tenCotNhom">Tiêu đề cột nhóm (vd "Trạng thái")</param>
    /// <param name="moTaBoLoc">Chuỗi mô tả điều kiện lọc in dưới tiêu đề báo cáo</param>
    Task ExportExcelAsync(TongHopTaiSanFilterDto filter, string tenCotNhom, string moTaBoLoc, string filePath);
}