using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ITAM.WPF.Converters
{
    /// <summary>
    /// null -> Visible, khác null -> Collapsed. Dùng cho placeholder hiển thị khi Content chưa có gì.
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}