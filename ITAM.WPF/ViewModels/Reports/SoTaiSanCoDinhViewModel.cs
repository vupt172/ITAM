using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ITAM.WPF.ViewModels.Reports
{
    // TODO xác nhận: đổi base class nếu BaoCaoTaiSanViewModel không dùng ObservableObject
    public partial class SoTaiSanCoDinhViewModel : ObservableObject
    {
        private readonly ISoTaiSanCoDinhService _service;

        public IReadOnlyList<int> DanhSachNam { get; }

        [ObservableProperty]
        private int namChon;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(XuatExcelCommand))]
        private bool isBusy;

        public SoTaiSanCoDinhViewModel(ISoTaiSanCoDinhService service)
        {
            _service = service;

            int namHienTai = DateTime.Now.Year;
            DanhSachNam = Enumerable.Range(2015, namHienTai - 2015 + 1).Reverse().ToList();
            NamChon = namHienTai;
        }
      
        private bool CanXuatExcel() => !IsBusy;

        [RelayCommand(CanExecute = nameof(CanXuatExcel))]
        private async Task XuatExcelAsync()
        {
            // TODO xác nhận: nếu BaoCaoTaiSanViewModel đã có helper lưu file thì thay đoạn SaveFileDialog này
            var dlg = new SaveFileDialog
            {
                Filter = "Excel (*.xlsx)|*.xlsx",
                FileName = $"SoTSCD_{NamChon}.xlsx"
            };
            if (dlg.ShowDialog() != true) return;

            try
            {
                IsBusy = true;
                var bytes = await _service.XuatAsync(NamChon);
                await File.WriteAllBytesAsync(dlg.FileName, bytes);
                MessageBox.Show("Xuất Sổ Tài Sản Cố Định thành công.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể xuất báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}