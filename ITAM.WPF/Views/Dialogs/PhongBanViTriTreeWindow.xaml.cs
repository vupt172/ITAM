using ITAM.ViewModels;
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
using System.Windows.Shapes;

namespace ITAM.WPF.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for PhongBanViTriTreeWindow.xaml
    /// </summary>
    // PhongBanViTriTreeWindow.xaml.cs
    public partial class PhongBanViTriTreeWindow : Window
    {
        public PhongBanViTriTreeWindow(PhongBanViTriTreeViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Loaded += async (_, _) => await vm.InitializeAsync();
        }
    }
}
