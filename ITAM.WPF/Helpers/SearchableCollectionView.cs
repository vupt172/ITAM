
using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.AppCore.DTOs;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ITAM.WPF.Helpers
{
    /// <summary>
    /// Bọc một nguồn dữ liệu thành ICollectionView riêng,
    /// hỗ trợ tìm kiếm autocomplete theo nhiều thuộc tính.
    ///
    /// Ví dụ:
    /// - Tìm phòng ban theo Code hoặc Name.
    /// - Tìm tài sản theo Code, Name hoặc SerialNumber.
    /// - Tìm nhà cung cấp theo Code, Name hoặc PhoneNumber.
    /// </summary>
    /// <typeparam name="T">Kiểu phần tử trong nguồn dữ liệu.</typeparam>
    public partial class SearchableCollectionView<T> : ObservableObject
    {
        private readonly ICollectionView _view;
        private readonly IReadOnlyList<Func<T, string?>> _searchSelectors;

        // Dùng khi thay đổi SearchText nhưng không muốn Refresh().
        private bool _suppressFilterUpdate;

        [ObservableProperty]
        private string? searchText;

        /// <summary>
        /// ICollectionView riêng của helper.
        /// Dùng để bind vào ItemsSource của ComboBox.
        /// </summary>
        public ICollectionView View => _view;

        /// <summary>
        /// Khởi tạo SearchableCollectionView.
        /// </summary>
        /// <param name="source">
        /// Nguồn dữ liệu. Khuyến nghị ObservableCollection&lt;T&gt;
        /// nếu danh sách có thể thay đổi trong runtime.
        /// </param>
        /// <param name="searchSelectors">
        /// Các thuộc tính dùng để tìm kiếm.
        ///
        /// Ví dụ:
        /// x => x.Code,
        /// x => x.Name
        /// </param>
        public SearchableCollectionView(
            IEnumerable<T> source,
            params Func<T, string?>[] searchSelectors)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (searchSelectors == null || searchSelectors.Length == 0)
            {
                throw new ArgumentException(
                    "Cần truyền ít nhất một selector để tìm kiếm.",
                    nameof(searchSelectors));
            }

            _searchSelectors = searchSelectors;

            // Tạo CollectionView riêng cho helper này.
            // Không dùng CollectionViewSource.GetDefaultView(source)
            // để tránh các ComboBox dùng chung source ảnh hưởng filter lẫn nhau.
            var collectionViewSource = new CollectionViewSource
            {
                Source = source
            };

            _view = collectionViewSource.View;

            _view.Filter = FilterItem;
        }

        /// <summary>
        /// Được CommunityToolkit.Mvvm source generator gọi
        /// khi SearchText thay đổi.
        /// </summary>
        partial void OnSearchTextChanged(string? value)
        {
            if (_suppressFilterUpdate)
                return;

            _view.Refresh();
        }

        /// <summary>
        /// Kiểm tra một item có phù hợp với SearchText hay không.
        /// Item được giữ lại nếu bất kỳ selector nào chứa từ khóa.
        /// </summary>
        private bool FilterItem(object obj)
        {
            // Không nhập từ khóa => hiển thị toàn bộ.
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            if (obj is not T item)
                return false;

            var keyword = SearchText.Trim();

            return _searchSelectors.Any(selector =>
            {
                var value = selector(item);

                return !string.IsNullOrWhiteSpace(value)
                    && value.Contains(
                        keyword,
                        StringComparison.CurrentCultureIgnoreCase);
            });
        }

        /// <summary>
        /// Gán text hiển thị mà không kích hoạt filter.
        ///
        /// Dùng trong trường hợp ComboBox đã chọn item và cần
        /// đưa tên/code của item lên ô nhập liệu.
        /// </summary>
        public void SetDisplayTextSilently(string? value)
        {
            _suppressFilterUpdate = true;

            try
            {
                SearchText = value;
            }
            finally
            {
                _suppressFilterUpdate = false;
            }
        }

        /// <summary>
        /// Xóa text tìm kiếm và hiển thị lại toàn bộ danh sách.
        /// </summary>
        public void ClearSearch()
        {
            SearchText = null;
        }

        /// <summary>
        /// Refresh lại ICollectionView.
        ///
        /// Dùng khi dữ liệu bên trong item thay đổi hoặc
        /// khi cần áp dụng lại filter thủ công.
        /// </summary>
        public void Refresh()
        {
            _view.Refresh();
        }
    }
}
