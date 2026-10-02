using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.AppCore.Interfaces;
using ITAM.WPF.Constants;
using ITAM.WPF.Models;
using ITAM.WPF.Services.Interfaces;
using ITAM.WPF.ViewModels.Reports;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace ITAM.WPF.ViewModels
{
    public partial class HeThongBaoCaoViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        public override string Title => PageTitles.HeThongBaoCao; // TODO xác nhận: thêm hằng số này vào PageTitles

        public ObservableCollection<CatalogNode> Catalogs { get; }

        [ObservableProperty]
        private CatalogNode? selectedCatalog;

        [ObservableProperty]
        private object? currentView;

        public HeThongBaoCaoViewModel(IServiceProvider serviceProvider, INavigationService navigationService, ICurrentUserContext currentUserContext)
            : base(navigationService, currentUserContext)
        {
            _serviceProvider = serviceProvider;

            Catalogs =
            [
                new()
                {
                    Title = "Tài Sản",
                    IsExpanded = true,
                    Children =
                    [
                        new() { Title = "Tổng Hợp Tài Sản",ViewModelType=typeof(TongHopTaiSanViewModel) },
                        new() { Title = "Danh Sách Tài Sản", ViewModelType = typeof(BaoCaoTaiSanViewModel) },
                        new() { Title = "Khấu Hao Tài Sản Cố Định", ViewModelType = typeof(SoTaiSanCoDinhViewModel) },
                        new() { Title = "Báo Cáo Nhập Tài Sản" },                          // chưa xây
                        new() { Title = "Báo Cáo Điều Chuyển Tài Sản" },                   // chưa xây
                    ]
                },
                new()
                {
                    Title = "Vật Tư",
                    IsExpanded = true,
                    Children =
                    [
                        new() { Title = "Báo Cáo Tồn Kho" },                               // chưa xây
                        new() { Title = "Báo Cáo Nhập - Xuất Vật Tư" },                     // chưa xây
                    ]
                },
            ];
        }

        protected override void InitToolbarState()
        {
            CanClose = true;
            CanRefresh = false;
        }

        partial void OnSelectedCatalogChanged(CatalogNode? value)
        {
            if (value?.ViewModelType == null)
            {
                CurrentView = null;
                return;
            }

            var view = _serviceProvider.GetRequiredService(value.ViewModelType);
            CurrentView = view;

            _ = LoadCurrentViewAsync(view);
        }

        private async Task LoadCurrentViewAsync(object view)
        {
            try
            {
                if (view is BaoCaoTaiSanViewModel bc)
                    await bc.InitializeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải dữ liệu báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}