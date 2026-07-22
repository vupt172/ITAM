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
    public partial class HeThongDMViewModel : BaseViewModel,IToolbarAware
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly INavigationService _navigationService;

        public override string Title => PageTitles.HeThongDM;
        public ToolbarState ToolbarState { get; }

        public ObservableCollection<CatalogNode> Catalogs { get; }

        [ObservableProperty]
        private CatalogNode? selectedCatalog;

        [ObservableProperty]
        private object? currentView;

        public HeThongDMViewModel(IServiceProvider serviceProvider,INavigationService navigationService)
        {
            _serviceProvider = serviceProvider;
            _navigationService = navigationService;

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

            ToolbarState = new ToolbarState
            {
                AddCommand = AddCommand,
                EditCommand = EditCommand,
                DeleteCommand = DeleteCommand,
                SaveCommand = SaveCommand,
                CancelCommand = CancelCommand,
                CloseCommand = CloseCommand
            };
        }
        

        partial void OnSelectedCatalogChanged(CatalogNode? value)
        {
            if (value == null || !value.IsLeaf)
                return;

            switch (value.Title)
            {
                case "Loại Tài Sản":
                    CurrentView = _serviceProvider.GetRequiredService<LoaiTaiSanViewModel>();
                    break;

                case "Danh Mục Tài Sản":
                    CurrentView = _serviceProvider.GetRequiredService<DMTaiSanViewModel>();
                    break;
            }
        }


        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        private bool canAdd=true;

        [RelayCommand(CanExecute = nameof(CanAdd))]
        private void Add()
        {
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditCommand))]
        private bool canEdit=true;

        [RelayCommand(CanExecute = nameof(CanEdit))]
        private void Edit()
        {
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
        private bool canDelete=true;

        [RelayCommand(CanExecute = nameof(CanDelete))]
        private void Delete()
        {
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private bool canSave;

        [RelayCommand(CanExecute = nameof(CanSave))]
        private void Save()
        {
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
        private bool canCancel;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel()
        {
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CloseCommand))]
        private bool canClose=true;

        [RelayCommand(CanExecute = nameof(CanClose))]
        private void Close()
        {
            _navigationService.CloseView(this);
        }
    }
}

