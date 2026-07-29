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
    public class DMTaiSanViewModel : CatalogViewModel
    {
        private readonly ICatalogService<DMTaiSan> _dmTaiSanService;
        public ToolbarContext ToolbarState { get; set; }

        protected override bool HasSelection => throw new NotImplementedException();

        public DMTaiSanViewModel(ICatalogService<DMTaiSan> dmTaiSanService)
        {
            _dmTaiSanService = dmTaiSanService;
        }

        public override Task LoadAsync()
        {
            throw new NotImplementedException();
        }
    }
}
