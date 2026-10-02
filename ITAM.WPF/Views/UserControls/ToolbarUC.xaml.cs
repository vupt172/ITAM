using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ITAM.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ToolbarUC.xaml
    /// </summary>
    public partial class ToolbarUC : UserControl
    {
        public ToolbarUC()
        {
            InitializeComponent();
        }
        private void BtnNghiepVu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { ContextMenu: { } menu } button)
            {
                menu.PlacementTarget = button;
                menu.Placement = PlacementMode.Bottom;
                menu.IsOpen = true;
            }
        }
    }
}
