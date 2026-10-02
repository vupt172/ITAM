using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.AppCore.Interfaces
{
    public interface ISoTaiSanCoDinhService
    {
        /// <summary>Xuất Sổ TSCĐ (mẫu S24-H) của năm chỉ định, trả về nội dung file .xlsx.</summary>
        Task<byte[]> XuatAsync(int nam);
    }
}
