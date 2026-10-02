using ITAM.AppCore.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.AppCore.Interfaces
{
    public interface IBaoCaoTaiSanService
    {
        Task<List<BaoCaoTaiSanDto>> TimKiemAsync(BaoCaoTaiSanSearchDto dto);
    }
}
