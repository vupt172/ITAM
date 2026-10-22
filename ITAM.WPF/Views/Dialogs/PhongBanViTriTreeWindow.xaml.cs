
using ITAM.ViewModels.Dialogs;
using System.Windows;


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
