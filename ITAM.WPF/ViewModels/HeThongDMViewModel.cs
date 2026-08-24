using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.Common;
using ITAM.AppCore.Interfaces;
using ITAM.WPF.Constants;
using ITAM.WPF.Models;
using ITAM.WPF.Services.Interfaces;
using ITAM.WPF.ViewModels.Catalogs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private CatalogViewModel? currentView;

        // Kế thừa contructor từ BaseViewModel sẽ tự gọi InitToolbarState() để khởi tạo trạng thái toolbar và property _navigationService
        public HeThongDMViewModel(IServiceProvider serviceProvider, IToolbarService toolbarService, INavigationService navigationService) : base(navigationService)
        {
            _serviceProvider = serviceProvider;
            _toolbarService = toolbarService;
            Catalogs =
[
    new()
    {
        Title = "Tài Sản",
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
                Title = "Nhà Cung Cấp",
                ViewModelType = typeof(NhaCungCapViewModel)
            }
];
        }

        protected override void InitToolbarState()
        {
            CanClose = true;
            CanRefresh = true;
        }

        partial void OnSelectedCatalogChanged(CatalogNode? value)
        {
            if (value?.ViewModelType == null)
                return;

            CurrentView = (CatalogViewModel)_serviceProvider.GetRequiredService(value.ViewModelType);
            AuthorizeCommands(CurrentView);
        }

        private void AuthorizeCommands(CatalogViewModel catalogViewModel)
        {
            ToolbarContext.AddCommand = catalogViewModel.AddCommand;
            ToolbarContext.EditCommand = catalogViewModel.EditCommand;
            ToolbarContext.DeleteCommand = catalogViewModel.DeleteCommand;
            ToolbarContext.SaveCommand = catalogViewModel.SaveCommand;
            ToolbarContext.CancelCommand = catalogViewModel.CancelCommand;
        }

    }
}

