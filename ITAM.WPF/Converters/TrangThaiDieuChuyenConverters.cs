using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ITAM.WPF.Converters
{
    public class TrangThaiDieuChuyenToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() switch
            {
                "APPROVED" => new SolidColorBrush(Color.FromRgb(0x2E, 0x7D, 0x32)),
                "REJECTED" => new SolidColorBrush(Color.FromRgb(0xD3, 0x2F, 0x2F)),
                _ => new SolidColorBrush(Color.FromRgb(0xF5, 0x7C, 0x00)) // PENDING
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class TrangThaiDieuChuyenToIconKindConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() switch
            {
                "APPROVED" => PackIconKind.CheckCircleOutline,
                "REJECTED" => PackIconKind.CloseCircleOutline,
                _ => PackIconKind.ClockOutline // PENDING
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}