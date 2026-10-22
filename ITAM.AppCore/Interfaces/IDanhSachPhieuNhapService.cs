using ITAM.AppCore.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.AppCore.Interfaces
{
    public interface IDanhSachPhieuNhapService
    {
        Task<PhieuNhapLookupDto> GetLookupAsync();
        Task<List<DanhSachPhieuNhapRowDto>> GetAsync(DanhSachPhieuNhapFilterDto filter);
        Task ExportExcelAsync(DanhSachPhieuNhapFilterDto filter, string moTaBoLoc, string filePath);
    }
}
