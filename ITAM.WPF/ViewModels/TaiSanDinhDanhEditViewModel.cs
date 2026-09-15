using AutoUpdaterDotNET;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.WPF.Services.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace ITAM.WPF.ViewModels
{
    public partial class TaiSanDinhDanhEditViewModel : ObservableValidator
    {
        private readonly ITaiSanDinhDanhService _taiSanDinhDanhService;
        private readonly IErrorDialogService _errorDialogService;

        private long _id;

        // Context — chỉ hiển thị, không cho sửa
        [ObservableProperty] private string code = string.Empty;
        [ObservableProperty] private string tenHangHoa = string.Empty;
        [ObservableProperty] private string tenLoaiTaiSan = string.Empty;
        [ObservableProperty] private string trangThai = string.Empty;
        [ObservableProperty] private string tenViTri = string.Empty;

        // Cho phép sửa
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Tên tài sản không được để trống.")]
        private string name = string.Empty;
        [ObservableProperty]
        private string? serial;
        [ObservableProperty]
        private int? namSuDung;
        [ObservableProperty]
        private string? ghiChu;

        [ObservableProperty] private bool isLoading;

        public event EventHandler<bool>? RequestClose;

        public TaiSanDinhDanhEditViewModel(
            ITaiSanDinhDanhService taiSanDinhDanhService,
            IErrorDialogService errorDialogService)
        {
            _taiSanDinhDanhService = taiSanDinhDanhService;
            _errorDialogService = errorDialogService;
        }

        public async Task InitializeAsync(long id)
        {
            IsLoading = true;
            try
            {
                _id = id;
                var ts = await _taiSanDinhDanhService.GetByIdAsync(id);
                if (ts == null) return;

                Code = ts.Code;
                Name = ts.Name;
                Serial = ts.Serial;
                TenHangHoa = ts.HangHoa?.Name ?? string.Empty;
                TenLoaiTaiSan = ts.LoaiTaiSan?.Name ?? string.Empty;
                TrangThai = ts.TrangThaiTaiSan.ToString();
                TenViTri = ts.ViTriTaiSan?.Name ?? string.Empty;
                NamSuDung = ts.NamSuDung;
                GhiChu = ts.GhiChu;
            }
            catch (Exception ex) { _errorDialogService.Show(ex); }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        private async Task Save()
        {
            ValidateAllProperties();
            if (HasErrors) return;

            try
            {
                await _taiSanDinhDanhService.UpdateAsync(new UpdateTaiSanDinhDanhDto
                {
                    Id = _id,
                    Name = Name,
                    Serial = Serial,
                    NamSuDung = NamSuDung,
                    GhiChu = GhiChu
                });
                RequestClose?.Invoke(this, true);
            }
            catch (Exception ex) { _errorDialogService.Show(ex); }
        }

        [RelayCommand]
        private void Cancel() => RequestClose?.Invoke(this, false);
    }
}