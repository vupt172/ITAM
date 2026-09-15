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
    public partial class VatTuEditViewModel : ObservableValidator
    {
        private readonly IVatTuService _vatTuService;
        private readonly IErrorDialogService _errorDialogService;

        private long _id;

        // Context — chỉ hiển thị
        [ObservableProperty] private string tenHangHoa = string.Empty;
        [ObservableProperty] private int soLuongTon;
        [ObservableProperty] private string tenViTri = string.Empty;

        // Cho phép sửa
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Tên vật tư không được để trống.")]
        private string name = string.Empty;
        [ObservableProperty]
        private string? donViTinh;
        [ObservableProperty]
        private string? ghiChu;

        [ObservableProperty] private bool isLoading;

        public event EventHandler<bool>? RequestClose;

        public VatTuEditViewModel(
            IVatTuService vatTuService,
            IErrorDialogService errorDialogService)
        {
            _vatTuService = vatTuService;
            _errorDialogService = errorDialogService;
        }

        public async Task InitializeAsync(long id)
        {
            IsLoading = true;
            try
            {
                _id = id;
                var vt = await _vatTuService.GetByIdAsync(id);
                if (vt == null) return;

                Name = vt.Name;
                DonViTinh = vt.DonViTinh;
                TenHangHoa = vt.HangHoa?.Name ?? string.Empty;
                SoLuongTon = vt.SoLuongTon;
                GhiChu=vt.GhiChu;
                TenViTri = vt.ViTriTaiSan?.Name ?? string.Empty;
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
                await _vatTuService.UpdateAsync(new UpdateVatTuDto
                {
                    Id = _id,
                    Name = Name,
                    DonViTinh = DonViTinh,
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