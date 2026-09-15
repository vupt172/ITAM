using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.AppCore.Common;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Exceptions;
using ITAM.WPF.Services.Interfaces;
using ITAM.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ITAM.WPF.ViewModels
{
    public partial class TaiSanDinhDanhListViewModel : BaseViewModel
    {
        private readonly ITaiSanDinhDanhService _taiSanDinhDanhService;
        private readonly IErrorDialogService _errorDialogService;
        private readonly ICurrentUserContext _currentUserContext;
        public override string Title => "Danh Sách Tài Sản Định Danh";

        [ObservableProperty] private ObservableCollection<TaiSanDinhDanhListDto> danhSach = new();
        [ObservableProperty] private TaiSanDinhDanhListDto? selectedItem;

        protected override bool HasSelection => SelectedItem != null;

        public TaiSanDinhDanhListViewModel(
            ITaiSanDinhDanhService taiSanDinhDanhService,
            IErrorDialogService errorDialogService,
            ICurrentUserContext currentUserContext,
            INavigationService navigationService) : base(navigationService)
        {
            _taiSanDinhDanhService = taiSanDinhDanhService;
            _errorDialogService = errorDialogService;
            _currentUserContext = currentUserContext;
            _ = LoadAsync();
        }

        protected override void InitToolbarState()
        {
            CanRefresh = true;
            CanClose = true;
        }
        protected override bool CanAdd() => false; // Không cho phép thêm mới từ màn hình này
        protected override bool CanDelete() => false; // Không cho phép xóa từ màn hình này

        private async Task LoadAsync()
        {
            try
            {
                var phongBanId = _currentUserContext.Instance?.PhongBanId
                    ?? throw new InvalidBusinessRuleException("Tài khoản chưa được gán Phòng Ban mặc định.");

                var list = await _taiSanDinhDanhService.GetAllAsync(phongBanId);
                DanhSach = new ObservableCollection<TaiSanDinhDanhListDto>(list.Select(x => new TaiSanDinhDanhListDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Serial = x.Serial,
                    MaHangHoa = x.HangHoa?.Code ?? string.Empty,
                    TenLoaiTaiSan = x.LoaiTaiSan?.Name ?? string.Empty,
                    TrangThaiTaiSan = x.TrangThaiTaiSan.ToString(),
                    TenViTri = x.ViTriTaiSan?.Name ?? string.Empty,
                    NamSuDung = x.NamSuDung,
                    GiaNhap = x.GiaNhap,
                    GhiChu = x.GhiChu,
                    SoLoNhapChiTiet = x.LoNhapChiTiet?.SoLo ?? string.Empty
                }));
            }
            catch (Exception ex) { _errorDialogService.Show(ex); }
        }

        protected override async void Refresh() => await LoadAsync();

        partial void OnSelectedItemChanged(TaiSanDinhDanhListDto? value) => NotifySelectionChanged();

        protected override void Edit()
        {
            if (SelectedItem == null) return;
            _ = OpenEditDialogAsync(SelectedItem.Id);
        }

        private async Task OpenEditDialogAsync(long id)
        {
            try
            {
                var dialogVm = App.Services.GetRequiredService<TaiSanDinhDanhEditViewModel>();
                var window = App.Services.GetRequiredService<TaiSanDinhDanhEditWindow>();

                window.DataContext = dialogVm;
                window.Owner = Application.Current.MainWindow;

                dialogVm.RequestClose += (s, saved) =>
                {
                    window.DialogResult = saved;
                    window.Close();
                };

                await dialogVm.InitializeAsync(id);

                if (window.ShowDialog() == true)
                    await LoadAsync();
            }
            catch (Exception ex) { _errorDialogService.Show(ex); }
        }
    }
}