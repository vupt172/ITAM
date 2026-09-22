using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Enums;
using ITAM.WPF.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ITAM.WPF.ViewModels
{
    public partial class DieuChuyenSearchViewModel : ObservableObject
    {
        private readonly IDieuChuyenService _dieuChuyenService;
        private readonly IErrorDialogService _errorDialogService;

        [ObservableProperty] private string? soPhieu;
        [ObservableProperty] private DateTime? tuNgay;
        [ObservableProperty] private DateTime? denNgay;
        [ObservableProperty] private string? selectedTrangThai; // "" = tất cả
        [ObservableProperty] private DieuChuyenFormDto? selectedResult;
        [ObservableProperty] private bool isLoading;

        public string[] TrangThaiOptions { get; } = { "", "PENDING", "APPROVED", "REJECTED" };
        public ObservableCollection<DieuChuyenFormDto> Results { get; } = new();

        public long? SelectedDieuChuyenId => SelectedResult?.Id;

        public event EventHandler<bool>? RequestClose;

        public DieuChuyenSearchViewModel(IDieuChuyenService dieuChuyenService, IErrorDialogService errorDialogService)
        {
            _dieuChuyenService = dieuChuyenService;
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
                var list = await _dieuChuyenService.TimKiemAsync(new DieuChuyenSearchDto
                {
                    SoPhieu = SoPhieu,
                    TuNgay = TuNgay,
                    DenNgay = DenNgay,
                    TrangThai = string.IsNullOrEmpty(SelectedTrangThai)
                        ? null
                        : Enum.Parse<TrangThaiDieuChuyen>(SelectedTrangThai)
                });

                Results.Clear();
                foreach (var x in list)
                    Results.Add(new DieuChuyenFormDto
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
            catch (Exception ex) { _errorDialogService.Show(ex); }
            finally { IsLoading = false; }
        }

        [RelayCommand(CanExecute = nameof(CanChon))]
        private void Chon() => RequestClose?.Invoke(this, true);
        private bool CanChon() => SelectedResult != null;

        [RelayCommand]
        private void Cancel() => RequestClose?.Invoke(this, false);

        partial void OnSelectedResultChanged(DieuChuyenFormDto? value) => ChonCommand.NotifyCanExecuteChanged();
    }
}