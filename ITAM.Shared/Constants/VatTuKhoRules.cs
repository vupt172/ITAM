using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Shared.Constants
{
    public static class VatTuKhoRules
    {
        public static readonly string[] PhongBanBiChan =
            { PhongBanCodes.KHO_THAT_LAC, PhongBanCodes.KHO_CHO_THANH_LY, PhongBanCodes.KHO_THANH_LY };

        public static readonly string[] ViTriBiChan =
            { ViTriTaiSanCodes.KHO_THAT_LAC, ViTriTaiSanCodes.KHO_CHO_THANH_LY, ViTriTaiSanCodes.KHO_THANH_LY };
    }
}
