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
    public record TrangThaiPhieuOption(string Ten, TrangThaiPhieu? GiaTri);

    public record PhieuThatLacListItem(
        long Id, string SoPhieu, DateTime NgayTao, string TenPhongBan, string TenNguoiTao, string TrangThai);

    public partial class PhieuThatLacSearchViewModel : ObservableObject
    {
        private readonly IPhieuThatLacService _service;

        public event EventHandler<bool>? RequestClose;

        public IReadOnlyList<TrangThaiPhieuOption> TrangThaiOptions { get; } = new[]
        {
            new TrangThaiPhieuOption("Tất cả", null),
            new TrangThaiPhieuOption("PENDING", TrangThaiPhieu.PENDING),
            new TrangThaiPhieuOption("APPROVED", TrangThaiPhieu.APPROVED),
            new TrangThaiPhieuOption("REJECTED", TrangThaiPhieu.REJECTED)
        };

        public ObservableCollection<PhieuThatLacListItem> KetQua { get; } = [];

        [ObservableProperty] private string? soPhieu;
        [ObservableProperty] private DateTime? tuNgay;
        [ObservableProperty] private DateTime? denNgay;
        [ObservableProperty] private TrangThaiPhieuOption trangThaiChon;
        [ObservableProperty] private PhieuThatLacListItem? dongChon;

        public long? SelectedPhieuThatLacId { get; private set; }

        public PhieuThatLacSearchViewModel(IPhieuThatLacService service)
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
                var ds = await _service.TimKiemAsync(new PhieuThatLacSearchDto
                {
                    SoPhieu = SoPhieu,
                    TuNgay = TuNgay?.Date,
                    // DatePicker trả 00:00 nên lấy hết ngày cuối, tránh mất phiếu tạo trong ngày đó.
                    DenNgay = DenNgay?.Date.AddDays(1).AddTicks(-1),
                    TrangThai = TrangThaiChon.GiaTri
                });

                KetQua.Clear();
                foreach (var p in ds)
                {
                    KetQua.Add(new PhieuThatLacListItem(
                        p.Id,
                        p.SoPhieu,
                        p.NgayTao,
                        p.PhongBan?.Name ?? string.Empty,
                        p.NguoiTao?.FullName ?? string.Empty,
                        p.TrangThai.ToString()));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}");   // TODO xác nhận: DieuChuyenSearchViewModel đang báo lỗi bằng cách nào (IErrorDialogService?)
            }
        }

 

        [RelayCommand]
        private void Chon()
        {
            if (DongChon == null)
                return;

            SelectedPhieuThatLacId = DongChon.Id;
            RequestClose?.Invoke(this, true);
        }
    }
}