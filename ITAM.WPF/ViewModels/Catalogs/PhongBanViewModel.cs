using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public partial class PhongBanViewModel 
    {
        private readonly ICatalogService<PhongBan> _phongBanService;
        public ToolbarContext ToolbarState { get; set; }

        public PhongBanViewModel(ICatalogService<PhongBan> phongBanService)
        {
            //_phongBanService = phongBanService;

            //ToolbarState = new ToolbarState
            //{
            //    AddCommand = AddCommand,
            //    EditCommand = EditCommand,
            //    DeleteCommand = DeleteCommand,
            //    SaveCommand = SaveCommand,
            //    CancelCommand = CancelCommand,
            //    CloseCommand = CloseCommand
            //};
        }


    }
}
