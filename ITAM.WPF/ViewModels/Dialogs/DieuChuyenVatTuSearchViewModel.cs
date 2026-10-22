using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Enums;
using ITAM.WPF.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ITAM.WPF.ViewModels.Dialogs
{
    public partial class DieuChuyenVatTuSearchViewModel : ObservableObject
    {
        private readonly IDieuChuyenVatTuService _dieuChuyenVatTuService;
        private readonly IErrorDialogService _errorDialogService;

        [ObservableProperty] private string? soPhieu;
        [ObservableProperty] private DateTime? tuNgay;
        [ObservableProperty] private DateTime? denNgay;
        [ObservableProperty] private string? selectedTrangThai; // "" = tất cả
        [ObservableProperty] private DieuChuyenVatTuFormDto? selectedResult;
        [ObservableProperty] private bool isLoading;

        public string[] TrangThaiOptions { get; } = { "", "PENDING", "APPROVED", "REJECTED" };
        public ObservableCollection<DieuChuyenVatTuFormDto> Results { get; } = new();

        public long? SelectedDieuChuyenVatTuId => SelectedResult?.Id;

        public event EventHandler<bool>? RequestClose;

        public DieuChuyenVatTuSearchViewModel(
            IDieuChuyenVatTuService dieuChuyenVatTuService,
            IErrorDialogService errorDialogService)
        {
            _dieuChuyenVatTuService = dieuChuyenVatTuService;
            _errorDialogService = errorDialogService;

            var today = DateTime.Today;
            TuNgay = new DateTime(today.Year, today.Month, 1);
            DenNgay = TuNgay.Value.AddMonths(1).AddDays(-1);

            _ = SearchAsync();
        }

        [RelayCommand]
        private async Task Search() => await SearchAsync();

        private async Task SearchAsync()
        {
            IsLoading = true;
            try
            {
                var list = await _dieuChuyenVatTuService.TimKiemAsync(new DieuChuyenVatTuSearchDto
                {
                    SoPhieu = SoPhieu,
                    TuNgay = TuNgay,
                    DenNgay = DenNgay,
                    TrangThai = string.IsNullOrEmpty(SelectedTrangThai)
                        ? null
                        : Enum.Parse<TrangThaiPhieu>(SelectedTrangThai)
                });

                Results.Clear();
                foreach (var x in list)
                {
                    Results.Add(new DieuChuyenVatTuFormDto
                    {
                        Id = x.Id,
                        SoPhieu = x.SoPhieu,
                        NgayTao = x.NgayTao,
                        NguoiTaoId = x.NguoiTaoId,
                        TenNguoiTao = x.NguoiTao?.FullName ?? string.Empty,
                        TenPhongBanChuyenDi = x.PhongBanChuyenDi?.Name ?? string.Empty,
                        TenPhongBanChuyenDen = x.PhongBanChuyenDen?.Name ?? string.Empty,
                        GhiChu = x.GhiChu,
                        TrangThai = x.TrangThai.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanChon))]
        private void Chon() => RequestClose?.Invoke(this, true);
        private bool CanChon() => SelectedResult != null;

        [RelayCommand]
        private void Cancel() => RequestClose?.Invoke(this, false);

        partial void OnSelectedResultChanged(DieuChuyenVatTuFormDto? value) =>
            ChonCommand.NotifyCanExecuteChanged();
    }
}