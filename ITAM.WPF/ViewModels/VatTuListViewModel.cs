using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.AppCore.Common;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.WPF.Services.Interfaces;
using ITAM.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ITAM.WPF.ViewModels
{
    public partial class VatTuListViewModel : BaseViewModel
    {
        private readonly IVatTuService _vatTuService;
        private readonly IErrorDialogService _errorDialogService;

        public override string Title => "Danh Sách Vật Tư";

        [ObservableProperty] private ObservableCollection<VatTuListDto> danhSach = new();
        [ObservableProperty] private VatTuListDto? selectedItem;

        protected override bool HasSelection => SelectedItem != null;

        public VatTuListViewModel(
            IVatTuService vatTuService,
            IErrorDialogService errorDialogService,
            INavigationService navigationService) : base(navigationService)
        {
            _vatTuService = vatTuService;
            _errorDialogService = errorDialogService;
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
                var list = await _vatTuService.GetAllAsync();
                DanhSach = new ObservableCollection<VatTuListDto>(list.Select(x => new VatTuListDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    MaHangHoa = x.HangHoa?.Code ?? string.Empty,
                    DonViTinh = x.DonViTinh,
                    SoLuongTon = x.SoLuongTon,
                    GhiChu = x.GhiChu,
                    TenViTri = x.ViTriTaiSan?.Name ?? string.Empty
                }));
            }
            catch (Exception ex) { _errorDialogService.Show(ex); }
        }

        protected override async void Refresh() => await LoadAsync();

        partial void OnSelectedItemChanged(VatTuListDto? value) => NotifySelectionChanged();

        protected override void Edit()
        {
            if (SelectedItem == null) return;
            _ = OpenEditDialogAsync(SelectedItem.Id);
        }

        private async Task OpenEditDialogAsync(long id)
        {
            try
            {
                var dialogVm = App.Services.GetRequiredService<VatTuEditViewModel>();
                var window = App.Services.GetRequiredService<VatTuEditWindow>();

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