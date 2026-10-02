using ITAM.WPF.ViewModels;
using ITAM.WPF.ViewModels.Reports;
using System.ComponentModel;
using System.Windows.Controls;

namespace ITAM.WPF.Views.Reports;

public partial class TongHopTaiSanView : UserControl
{
    public TongHopTaiSanView()
    {
        InitializeComponent();
        DataContextChanged += (_, e) =>
        {
            if (e.OldValue is INotifyPropertyChanged oldVm) oldVm.PropertyChanged -= OnVmPropertyChanged;
            if (e.NewValue is INotifyPropertyChanged newVm)
            {
                newVm.PropertyChanged += OnVmPropertyChanged;
                ApplyHeader();
            }
        };
    }

    private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TongHopTaiSanViewModel.TenCotNhom)) ApplyHeader();
    }

    // DataGridColumn.Header không nằm trong visual tree nên không bind trực tiếp tới DataContext được
    private void ApplyHeader()
    {
        if (DataContext is TongHopTaiSanViewModel vm) NhomColumn.Header = vm.TenCotNhom;
    }
}