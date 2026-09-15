using System;
using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace ITAM.WPF.Converters
{
    /// <summary>Map TrangThai (PENDING/APPROVED/REJECTED - string) sang PackIconKind tương ứng.</summary>
    public class TrangThaiLoNhapToIconKindConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var trangThai = value as string;
            return trangThai switch
            {
                "PENDING" => PackIconKind.ClockOutline,
                "APPROVED" => PackIconKind.CheckCircle,
                "REJECTED" => PackIconKind.CloseCircle,
                _ => PackIconKind.HelpCircleOutline
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}