using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ITAM.AppCore.Common
{
    public partial class ToolbarContext : ObservableObject
    {
        [ObservableProperty] public ICommand? addCommand;
        [ObservableProperty] public ICommand? editCommand;
        [ObservableProperty] public ICommand? deleteCommand;
        [ObservableProperty] public ICommand? saveCommand;
        [ObservableProperty] public ICommand? cancelCommand;
        [ObservableProperty] public ICommand? closeCommand;
        [ObservableProperty] public ICommand? refreshCommand;
        [ObservableProperty] public ICommand? searchCommand;

        // Nghiệp vụ: không có mục nào thì nút tự ẩn
        public ObservableCollection<NghiepVuMenuItem> NghiepVuItems { get; } = new();

        public bool HasNghiepVu => NghiepVuItems.Count > 0;

        public ToolbarContext()
        {
            NghiepVuItems.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasNghiepVu));
        }

        public void SetNghiepVu(IEnumerable<NghiepVuMenuItem> items)
        {
            NghiepVuItems.Clear();
            foreach (var item in items) NghiepVuItems.Add(item);
        }

        public void ClearNghiepVu() => NghiepVuItems.Clear();
    }
}