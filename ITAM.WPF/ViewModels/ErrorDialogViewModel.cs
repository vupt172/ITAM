using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace ITAM.WPF.Views
{
    /// <summary>
    /// ViewModel cho ErrorDialog2 - hiển thị thông tin exception cho người dùng.
    /// Sử dụng CommunityToolkit.Mvvm (Nuget: CommunityToolkit.Mvvm).
    /// </summary>
    public partial class ErrorDialogViewModel : ObservableObject
    {
        [ObservableProperty]
        private string exceptionType = string.Empty;

        [ObservableProperty]
        private string source = string.Empty;

        [ObservableProperty]
        private string time = string.Empty;

        [ObservableProperty]
        private string message = string.Empty;

        [ObservableProperty]
        private string stackTrace = string.Empty;

        // Action được gán từ code-behind (Window) để ViewModel có thể yêu cầu đóng cửa sổ
        // mà không cần tham chiếu trực tiếp tới Window (giữ ViewModel "sạch", dễ test).
        public Action? RequestClose { get; set; }

        public ErrorDialogViewModel()
        {
        }

        /// <summary>
        /// Khởi tạo ViewModel trực tiếp từ một Exception.
        /// </summary>
        public ErrorDialogViewModel(Exception ex, string? source = null)
        {
            LoadFrom(ex, source);
        }

        /// <summary>
        /// Nạp thông tin từ Exception vào ViewModel.
        /// </summary>
        public void LoadFrom(Exception ex, string? source = null)
        {
            if (ex == null) throw new ArgumentNullException(nameof(ex));

            ExceptionType = ex.GetType().FullName ?? ex.GetType().Name;
            Source = source ?? ex.Source ?? "Không xác định";
            Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            Message = ex.Message;
            StackTrace = ex.ToString(); // Bao gồm cả inner exception, đầy đủ hơn ex.StackTrace
        }

        [RelayCommand]
        private void Copy()
        {
            try
            {
                var content =
                    $"Loại ngoại lệ : {ExceptionType}{Environment.NewLine}" +
                    $"Nguồn         : {Source}{Environment.NewLine}" +
                    $"Thời gian     : {Time}{Environment.NewLine}" +
                    $"Thông báo lỗi : {Message}{Environment.NewLine}{Environment.NewLine}" +
                    $"Stack Trace:{Environment.NewLine}{StackTrace}";

                System.Windows.Clipboard.SetText(content);
            }
            catch
            {
                // Clipboard có thể bị chiếm bởi tiến trình khác (COM exception),
                // bỏ qua để không làm crash chính dialog báo lỗi.
            }
        }

        [RelayCommand]
        private void Close()
        {
            RequestClose?.Invoke();
        }
    }
}