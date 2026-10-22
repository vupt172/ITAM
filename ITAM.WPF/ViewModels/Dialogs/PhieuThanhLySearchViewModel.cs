using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace ITAM.WPF.ViewModels.Dialogs
{
    public record PhieuThanhLyListItem(
        long Id, string SoPhieu, DateTime NgayTao, string LyDo, string TenNguoiTao, string TrangThai);

    public partial class PhieuThanhLySearchViewModel : ObservableObject
    {
        private readonly IPhieuThanhLyService _service;

        public event EventHandler<bool>? RequestClose;

        public IReadOnlyList<TrangThaiPhieuOption> TrangThaiOptions { get; } = new[]
        {
            new TrangThaiPhieuOption("Tất cả", null),
            new TrangThaiPhieuOption("PENDING", TrangThaiPhieu.PENDING),
            new TrangThaiPhieuOption("APPROVED", TrangThaiPhieu.APPROVED),
            new TrangThaiPhieuOption("REJECTED", TrangThaiPhieu.REJECTED)
        };

        public ObservableCollection<PhieuThanhLyListItem> KetQua { get; } = [];

        [ObservableProperty] private string? soPhieu;
        [ObservableProperty] private DateTime? tuNgay;
        [ObservableProperty] private DateTime? denNgay;
        [ObservableProperty] private TrangThaiPhieuOption trangThaiChon;
        [ObservableProperty] private PhieuThanhLyListItem? dongChon;

        public long? SelectedPhieuThanhLyId { get; private set; }

        public PhieuThanhLySearchViewModel(IPhieuThanhLyService service)
        {
            _service = service;
            trangThaiChon = TrangThaiOptions[0];

            // Mặc định lọc theo tháng hiện tại: từ ngày 1 đến ngày cuối tháng.
            var homNay = DateTime.Today;
            tuNgay = new DateTime(homNay.Year, homNay.Month, 1);
            denNgay = tuNgay.Value.AddMonths(1).AddDays(-1);
        }

        [RelayCommand]
        public async Task TimKiemAsync()
        {
            try
            {
                var ds = await _service.TimKiemAsync(new PhieuThanhLySearchDto
                {
                    SoPhieu = SoPhieu,
                    TuNgay = TuNgay?.Date,
                    DenNgay = DenNgay?.Date.AddDays(1).AddTicks(-1),
                    TrangThai = TrangThaiChon.GiaTri
                });

                KetQua.Clear();
                foreach (var p in ds)
                {
                    KetQua.Add(new PhieuThanhLyListItem(
                        p.Id, p.SoPhieu, p.NgayTao, p.LyDo,
                        p.NguoiTao?.FullName ?? string.Empty,
                        p.TrangThai.ToString()));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}");   // TODO xác nhận: giống PhieuThatLacSearchViewModel
            }
        }

        [RelayCommand]
        private void Chon()
        {
            if (DongChon == null)
                return;

            SelectedPhieuThanhLyId = DongChon.Id;
            RequestClose?.Invoke(this, true);
        }
    }
}