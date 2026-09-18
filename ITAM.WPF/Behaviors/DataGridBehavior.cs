using System.Windows;
using System.Windows.Controls;

namespace ITAM.WPF.Behaviors
{
    public static class DataGridBehavior
    {
        public static readonly DependencyProperty AutoIndexProperty =
            DependencyProperty.RegisterAttached(
                "AutoIndex",
                typeof(bool),
                typeof(DataGridBehavior),
                new PropertyMetadata(false, OnAutoIndexChanged));

        public static bool GetAutoIndex(DependencyObject obj) => (bool)obj.GetValue(AutoIndexProperty);
        public static void SetAutoIndex(DependencyObject obj, bool value) => obj.SetValue(AutoIndexProperty, value);

        private static void OnAutoIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid grid)
            {
                if ((bool)e.NewValue)
                    grid.LoadingRow += Grid_LoadingRow;
                else
                    grid.LoadingRow -= Grid_LoadingRow;
            }
        }

        private static void Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}