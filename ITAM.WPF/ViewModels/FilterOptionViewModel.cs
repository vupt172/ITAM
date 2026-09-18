using CommunityToolkit.Mvvm.ComponentModel;

namespace ITAM.WPF.ViewModels
{
    public partial class FilterOptionViewModel : ObservableObject
    {
        public string Value { get; }

        [ObservableProperty] private bool isChecked;

        public FilterOptionViewModel(string value, bool isChecked = false)
        {
            Value = value;
            this.isChecked = isChecked;
        }
    }
}