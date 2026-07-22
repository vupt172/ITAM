using ITAM.WPF.Models;
using ITAM.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ITAM.WPF.Views
{
    /// <summary>
    /// Interaction logic for HeThongDMView.xaml
    /// </summary>
    public partial class HeThongDMView : UserControl
    {
        public HeThongDMView()
        {
            InitializeComponent();
        }
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is HeThongDMViewModel vm)
            {
                vm.SelectedCatalog = e.NewValue as CatalogNode;
            }
        }
    }

}
