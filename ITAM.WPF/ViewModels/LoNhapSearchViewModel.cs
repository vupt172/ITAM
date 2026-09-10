using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Enums;
using ITAM.WPF.Services.Interfaces;
using System;
using System.Collections.ObjectModel;

namespace ITAM.WPF.ViewModels
{
    public partial class LoNhapSearchViewModel : ObservableObject
    {
        private readonly ILoNhapService _loNhapService;
        private readonly IErrorDialogService _errorDialogService;

        [ObservableProperty] private string? soLo;
        [ObservableProperty] private DateTime? tuNgay;
        [ObservableProperty] private DateTime? denNgay;
        [ObservableProperty] private string? selectedTrangThai; // "" = tất cả
        [ObservableProperty] private LoNhapFormDto? selectedResult;
        [ObservableProperty] private bool isLoading;

        public string[] TrangThaiOptions { get; } = { "", "PENDING", "APPROVED", "REJECTED" };
        public ObservableCollection<LoNhapFormDto> Results { get; } = new();

        public long? SelectedLoNhapId => SelectedResult?.Id;

        public event EventHandler<bool>? RequestClose;

        public LoNhapSearchViewModel(ILoNhapService loNhapService, IErrorDialogService errorDialogService)
        {
            _loNhapService = loNhapService;
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
                var list = await _loNhapService.TimKiemAsync(new LoNhapSearchDto
                {
                    SoLo = SoLo,
                    TuNgay = TuNgay,
                    DenNgay = DenNgay,
                    TrangThai = string.IsNullOrEmpty(SelectedTrangThai)
                        ? null
                        : Enum.Parse<TrangThaiLoNhap>(SelectedTrangThai)
                });

                Results.Clear();
                foreach (var x in list)
                    Results.Add(new LoNhapFormDto
                    {
                        Id = x.Id,
                        SoLo = x.SoLo,
                        NgayNhap = x.NgayNhap,
                        NhaCungCapId = x.NhaCungCapId,
                        TenNhaCungCap = x.NhaCungCap?.Name,
                        NguoiLapPhieuId = x.NguoiLapPhieuId,
                        TenNguoiLapPhieu = x.NguoiLapPhieu?.FullName ?? string.Empty,
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

        partial void OnSelectedResultChanged(LoNhapFormDto? value) => ChonCommand.NotifyCanExecuteChanged();
    }
}