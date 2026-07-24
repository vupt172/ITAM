using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.Common;
using ITAM.AppCore.Interfaces;
using ITAM.WPF.Constants;
using ITAM.WPF.Models;
using ITAM.WPF.Services;
using ITAM.WPF.ViewModels.Catalogs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace ITAM.WPF.ViewModels
{
    public partial class HeThongDMViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IToolbarService _toolbarService;
        public override string Title => PageTitles.HeThongDM;


        public ObservableCollection<CatalogNode> Catalogs { get; }

        [ObservableProperty]
        private CatalogNode? selectedCatalog;

        [ObservableProperty]
        private BaseViewModel? currentView;

        public IToolbarAware? CurrentToolbarTarget => CurrentView as IToolbarAware;

        public HeThongDMViewModel(IServiceProvider serviceProvider, INavigationService navigationService, IToolbarService toolbarService):base(navigationService)
        {
            _serviceProvider = serviceProvider;
            _toolbarService = toolbarService;

            Catalogs =
[
    new()
    {
        Title = "Danh Mục Tài Sản",
        Children =
        [
            new()
            {
                Title = "Loại Tài Sản",
                ViewModelType = typeof(LoaiTaiSanViewModel)
            },
            new()
            {
                Title = "Danh Mục Tài Sản",
                ViewModelType = typeof(DMTaiSanViewModel)
            },
            new()
            {
                Title = "Vị Trí Tài Sản",
                ViewModelType = typeof(ViTriTaiSanViewModel)
            }
        ]
    },

    new()
    {
        Title = "Tổ Chức",
        Children =
        [
            new()
            {
                Title = "Phòng Ban",
                ViewModelType = typeof(PhongBanViewModel)
            }
        ]
    },

    new()
    {
        Title = "Đối Tác",
        Children =
        [
            new()
            {
                Title = "Nhà Cung Cấp",
                ViewModelType = typeof(NhaCungCapViewModel)
            }
        ]
    }
];

            InitToolbarState();
        }

        protected override void InitToolbarState()
        {
            CanAdd = true;
            CanEdit = true;
            CanDelete = true;
            CanClose = true;
        }
   
        partial void OnSelectedCatalogChanged(CatalogNode? value)
        {
            if (value?.ViewModelType == null)
                return;

            CurrentView = (BaseViewModel)_serviceProvider.GetRequiredService(value.ViewModelType);
            //UpdateToolbar(CurrentView);
        }




    }
}

