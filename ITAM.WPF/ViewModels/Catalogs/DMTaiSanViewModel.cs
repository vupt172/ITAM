using ITAM.AppCore.Common;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities;
using ITAM.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.WPF.ViewModels.Catalogs
{
    public class DMTaiSanViewModel : BaseViewModel, IToolbarAware
    {
        private readonly ICatalogService<DMTaiSan> _dmTaiSanService;
        public ToolbarState ToolbarState { get; set; }

        public DMTaiSanViewModel(ICatalogService<DMTaiSan> dmTaiSanService)
        {
            _dmTaiSanService = dmTaiSanService;
        }



    }
}
