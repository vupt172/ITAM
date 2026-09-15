using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ITAM.WPF.Converters
{
    /// <summary>Map TrangThai (PENDING/APPROVED/REJECTED - string) sang màu nền badge.</summary>
    public class TrangThaiLoNhapToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var trangThai = value as string;
            return trangThai switch
            {
                "PENDING" => new SolidColorBrush(Color.FromRgb(0xFF, 0xB3, 0x00)),  // vàng cam
                "APPROVED" => new SolidColorBrush(Color.FromRgb(0x2E, 0x7D, 0x32)), // xanh lá
                "REJECTED" => new SolidColorBrush(Color.FromRgb(0xD3, 0x2F, 0x2F)), // đỏ
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}