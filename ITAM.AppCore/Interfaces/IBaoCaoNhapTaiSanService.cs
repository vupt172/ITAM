
using ITAM.AppCore.DTOs;

namespace ITAM.AppCore.Interfaces
{
    public interface IBaoCaoNhapTaiSanService
    {
        Task<NhapTaiSanLookupDto> GetLookupAsync();
        Task<List<BaoCaoNhapTaiSanRowDto>> GetAsync(BaoCaoNhapTaiSanFilterDto filter);

        /// <param name="moTaBoLoc">Chuỗi mô tả điều kiện lọc in dưới tiêu đề báo cáo</param>
        Task ExportExcelAsync(BaoCaoNhapTaiSanFilterDto filter, string moTaBoLoc, string filePath);
    }
}
