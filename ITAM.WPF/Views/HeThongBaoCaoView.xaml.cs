using ITAM.WPF.Models;
using ITAM.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ITAM.WPF.Views
{
    /// <summary>
    /// Interaction logic for HeThongBaoCaoView.xaml
    /// </summary>
    public partial class HeThongBaoCaoView : UserControl
    {
        public HeThongBaoCaoView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is HeThongBaoCaoViewModel vm)
            {
                vm.SelectedCatalog = e.NewValue as CatalogNode;
            }
        }
    }
}