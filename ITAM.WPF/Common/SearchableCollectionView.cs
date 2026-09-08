using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace ITAM.WPF.Common
{
    /// <summary>
    /// Bọc 1 ObservableCollection + ICollectionView + logic lọc theo SearchText,
    /// dùng chung cho mọi ComboBox/DataGrid cần tìm kiếm (autocomplete).
    /// HighlightedItem chỉ phục vụ hiển thị bôi đen — KHÔNG tự đồng bộ với
    /// selection "chính thức" của ViewModel để tránh tự chọn nhầm khi lọc.
    /// </summary>
    public class SearchableCollectionView<T> : INotifyPropertyChanged
    {
        public ObservableCollection<T> Items { get; } = new();
        public ICollectionView View { get; }

        private readonly Func<T, string, bool> _matchPredicate;
        private string? _searchText;
        private bool _suppressFilter;

        public string? SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                OnPropertyChanged();

                if (_suppressFilter) return;

                View.Refresh();
                View.MoveCurrentToFirst(); // chỉ đổi View.CurrentItem, không đụng SelectedUser
            }
        }

        // Item đang được bôi đen trong dropdown — bind 2 chiều với ComboBox.SelectedItem.
        // Đây KHÔNG phải là lựa chọn chính thức, chỉ dùng để hiển thị + làm nguồn cho Enter/Click xác nhận.
        public T? HighlightedItem
        {
            get => View.CurrentItem is T item ? item : default;
            set
            {
                if (value == null) View.MoveCurrentToPosition(-1);
                else View.MoveCurrentTo(value);
            }
        }

        public SearchableCollectionView(Func<T, string, bool> matchPredicate)
        {
            _matchPredicate = matchPredicate;
            View = CollectionViewSource.GetDefaultView(Items);
            View.Filter = Filter;
            View.CurrentChanged += (s, e) => OnPropertyChanged(nameof(HighlightedItem));
        }

        public void Reset(IEnumerable<T> newItems)
        {
            Items.Clear();
            foreach (var item in newItems)
                Items.Add(item);
            View.Refresh();
            View.MoveCurrentToFirst();
        }

        /// <summary>
        /// Gán text hiển thị SAU KHI đã chọn 1 item chính thức (vd "Họ tên - Tài khoản"),
        /// không kích hoạt filter lại.
        /// </summary>
        public void SetDisplayTextSilently(string? text)
        {
            _suppressFilter = true;
            SearchText = text;
            _suppressFilter = false;
        }

        private bool Filter(object obj)
        {
            if (obj is not T item) return false;
            if (string.IsNullOrWhiteSpace(SearchText)) return true;
            return _matchPredicate(item, SearchText);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}