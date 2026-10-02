using ITAM.WPF.Views.UserControls;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ITAM.WPF.Views.UserControls;

public partial class MultiColumnSearchComboBox : UserControl
{
    private ICollectionView? _view;

    private bool _isSelectingItem;
    private object? _previousSelectedItem;


    public MultiColumnSearchComboBox()
    {
        InitializeComponent();

        PART_DataGrid.SelectionChanged += DataGrid_SelectionChanged;

        PART_TextBox.GotFocus += TextBox_GotFocus;
        PART_TextBox.PreviewKeyDown += TextBox_PreviewKeyDown;

        Loaded += MultiColumnSearchComboBox_Loaded;
    }


    private void MultiColumnSearchComboBox_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        CreateColumns();
        CreateView();

        // Nếu SelectedItem đã có từ ViewModel
        // thì đồng bộ lại giao diện.
        if (SelectedItem != null)
        {
            UpdateDisplayFromSelectedItem();
        }
        // Trường hợp binding chỉ dùng SelectedValue (không dùng SelectedItem)
        // — vd. mở phiếu cũ đã có sẵn ViTriChuyenDenId — cũng cần đồng bộ lại.
        else if (SelectedValue != null)
        {
            SyncFromSelectedValue(SelectedValue);
        }
    }


    // ============================================================
    // ItemsSource
    // ============================================================

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(MultiColumnSearchComboBox),
            new PropertyMetadata(null, OnItemsSourceChanged));


    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }


    private static void OnItemsSourceChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiColumnSearchComboBox)d;

        control.CreateColumns();
        control.CreateView();

        // ItemsSource có thể được nạp SAU khi SelectedValue đã được gán
        // (vd. LoadDanhMucAsync chạy async) — đồng bộ lại hiển thị lúc này.
        if (control.SelectedItem == null && control.SelectedValue != null)
        {
            control.SyncFromSelectedValue(control.SelectedValue);
        }
    }

    private void DropDownButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        IsDropDownOpen = !IsDropDownOpen;

        if (IsDropDownOpen)
        {
            _view?.Refresh();

            PART_DataGrid.SelectedItem = SelectedItem;

            PART_TextBox.Focus();
        }
    }




    // ============================================================
    // SelectedItem
    // ============================================================

    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(
            nameof(SelectedItem),
            typeof(object),
            typeof(MultiColumnSearchComboBox),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSelectedItemChanged));


    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }


    private static void OnSelectedItemChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiColumnSearchComboBox)d;

        if (control._isSelectingItem)
            return;

        if (e.NewValue == null)
        {
            control.PART_DataGrid.SelectedItem = null;
            control.SearchText = string.Empty;
            control.SelectedValue = null;

            return;
        }

        control.PART_DataGrid.SelectedItem = e.NewValue;

        control.UpdateDisplayFromSelectedItem();
    }


    // ============================================================
    // SelectedValue
    // ============================================================

    public static readonly DependencyProperty SelectedValueProperty =
        DependencyProperty.Register(
            nameof(SelectedValue),
            typeof(object),
            typeof(MultiColumnSearchComboBox),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSelectedValueChanged));


    public object? SelectedValue
    {
        get => GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }


    /// <summary>
    /// Đồng bộ ngược khi SelectedValue bị đổi TỪ BÊN NGOÀI (binding từ ViewModel) —
    /// vd. ViewModel reset CurrentChiTiet = new() làm ViTriChuyenDenId về 0/null,
    /// hoặc load 1 phiếu cũ đã có sẵn giá trị. Không xử lý khi chính control này
    /// vừa set SelectedValue trong SelectItem() (tránh vòng lặp — canh bằng _isSelectingItem).
    /// </summary>
    private static void OnSelectedValueChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiColumnSearchComboBox)d;

        if (control._isSelectingItem)
            return;

        control.SyncFromSelectedValue(e.NewValue);
    }


    private void SyncFromSelectedValue(object? value)
    {
        // Không có giá trị, hoặc giá trị mặc định (0 với kiểu số) — coi như CHƯA CHỌN GÌ,
        // phải xóa hiển thị/selection cũ đi, không được để lại text của lần chọn trước.
        if (value == null || IsUnsetValue(value))
        {
            _isSelectingItem = true;

            try
            {
                SelectedItem = null;
                PART_DataGrid.SelectedItem = null;
                SearchText = string.Empty;
            }
            finally
            {
                _isSelectingItem = false;
            }

            return;
        }

        var match = FindItemBySelectedValue(value);

        _isSelectingItem = true;

        try
        {
            SelectedItem = match;
            PART_DataGrid.SelectedItem = match;

            if (match != null &&
                !string.IsNullOrWhiteSpace(DisplayMemberPath))
            {
                var displayValue =
                    GetPropertyValue(match, DisplayMemberPath);

                SearchText =
                    displayValue?.ToString() ?? string.Empty;
            }
        }
        finally
        {
            _isSelectingItem = false;
        }
    }


    private object? FindItemBySelectedValue(object value)
    {
        if (ItemsSource == null ||
            string.IsNullOrWhiteSpace(SelectedValuePath))
        {
            return null;
        }

        foreach (var item in ItemsSource)
        {
            var itemValue =
                GetPropertyValue(item, SelectedValuePath);

            if (Equals(itemValue, value))
                return item;
        }

        return null;
    }


    /// <summary>
    /// Coi 0 (int/long) là "chưa chọn" — khớp với cách các ViewModel trong dự án
    /// dùng giá trị mặc định của kiểu số (long PhongBanChuyenDenChonId, ...) thay vì null
    /// để đánh dấu chưa chọn gì.
    /// </summary>
    private static bool IsUnsetValue(object value)
    {
        return value switch
        {
            long l => l == 0,
            int i => i == 0,
            _ => false
        };
    }


    // ============================================================
    // SelectedValuePath
    // ============================================================

    public static readonly DependencyProperty SelectedValuePathProperty =
        DependencyProperty.Register(
            nameof(SelectedValuePath),
            typeof(string),
            typeof(MultiColumnSearchComboBox),
            new PropertyMetadata(string.Empty));


    public string SelectedValuePath
    {
        get => (string)GetValue(SelectedValuePathProperty);
        set => SetValue(SelectedValuePathProperty, value);
    }


    // ============================================================
    // DisplayMemberPath
    // ============================================================

    public static readonly DependencyProperty DisplayMemberPathProperty =
        DependencyProperty.Register(
            nameof(DisplayMemberPath),
            typeof(string),
            typeof(MultiColumnSearchComboBox),
            new PropertyMetadata(string.Empty));


    public string DisplayMemberPath
    {
        get => (string)GetValue(DisplayMemberPathProperty);
        set => SetValue(DisplayMemberPathProperty, value);
    }


    // ============================================================
    // SearchText
    // ============================================================

    public static readonly DependencyProperty SearchTextProperty =
        DependencyProperty.Register(
            nameof(SearchText),
            typeof(string),
            typeof(MultiColumnSearchComboBox),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSearchTextChanged));


    public string SearchText
    {
        get => (string)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }


    private static void OnSearchTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiColumnSearchComboBox)d;

        control._view?.Refresh();

        // Khi người dùng đang gõ
        // thì mở danh sách.
        if (!control._isSelectingItem &&
            control.IsKeyboardFocusWithin)
        {
            control.IsDropDownOpen = true;
        }
    }


    // ============================================================
    // Hint
    // ============================================================

    public static readonly DependencyProperty HintProperty =
        DependencyProperty.Register(
            nameof(Hint),
            typeof(string),
            typeof(MultiColumnSearchComboBox),
            new PropertyMetadata(string.Empty));


    public string Hint
    {
        get => (string)GetValue(HintProperty);
        set => SetValue(HintProperty, value);
    }


    // ============================================================
    // IsDropDownOpen
    // ============================================================

    public static readonly DependencyProperty IsDropDownOpenProperty =
        DependencyProperty.Register(
            nameof(IsDropDownOpen),
            typeof(bool),
            typeof(MultiColumnSearchComboBox),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


    public bool IsDropDownOpen
    {
        get => (bool)GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }


    // ============================================================
    // Columns
    // ============================================================

    public ObservableCollection<SearchColumn> Columns { get; } = new();


    // ============================================================
    // Create View
    // ============================================================

    private void CreateView()
    {
        if (ItemsSource == null)
        {
            _view = null;
            PART_DataGrid.ItemsSource = null;
            return;
        }

        var collectionViewSource = new CollectionViewSource
        {
            Source = ItemsSource
        };

        _view = collectionViewSource.View;

        _view.Filter = FilterItem;

        PART_DataGrid.ItemsSource = _view;
    }


    // ============================================================
    // Filter
    // ============================================================

    private bool FilterItem(object obj)
    {
        if (string.IsNullOrWhiteSpace(SearchText))
            return true;

        if (obj == null)
            return false;

        var keyword = SearchText.Trim();

        // Tìm theo DisplayMemberPath
        if (!string.IsNullOrWhiteSpace(DisplayMemberPath))
        {
            var displayValue =
                GetPropertyValue(obj, DisplayMemberPath);

            if (ContainsIgnoreCase(displayValue, keyword))
                return true;
        }

        // Tìm theo tất cả các cột
        foreach (var column in Columns)
        {
            var value =
                GetPropertyValue(obj, column.Binding);

            if (ContainsIgnoreCase(value, keyword))
                return true;
        }

        return false;
    }


    private static bool ContainsIgnoreCase(
        object? value,
        string keyword)
    {
        return value?
            .ToString()?
            .Contains(
                keyword,
                StringComparison.CurrentCultureIgnoreCase)
            == true;
    }


    // ============================================================
    // Create DataGrid Columns
    // ============================================================

    private void CreateColumns()
    {
        if (PART_DataGrid == null)
            return;

        PART_DataGrid.Columns.Clear();

        foreach (var column in Columns)
        {
            var dataGridColumn = new DataGridTextColumn
            {
                Header = column.Header,

                Binding = new Binding(column.Binding),

                Width = column.Width
            };

            PART_DataGrid.Columns.Add(dataGridColumn);
        }
    }


    // ============================================================
    // DataGrid Selection
    // ============================================================

    private void DataGrid_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (PART_DataGrid.SelectedItem == null)
            return;

        SelectItem(PART_DataGrid.SelectedItem);
    }


    private void SelectItem(object item)
    {
        _isSelectingItem = true;

        try
        {
            _previousSelectedItem = item;

            SelectedItem = item;

            // SelectedValue = item.Id
            if (!string.IsNullOrWhiteSpace(SelectedValuePath))
            {
                SelectedValue =
                    GetPropertyValue(
                        item,
                        SelectedValuePath);
            }

            // TextBox = item.Name
            if (!string.IsNullOrWhiteSpace(DisplayMemberPath))
            {
                var displayValue =
                    GetPropertyValue(
                        item,
                        DisplayMemberPath);

                SearchText =
                    displayValue?.ToString() ?? string.Empty;
            }

            IsDropDownOpen = false;
        }
        finally
        {
            _isSelectingItem = false;
        }
    }


    // ============================================================
    // Update display
    // ============================================================

    private void UpdateDisplayFromSelectedItem()
    {
        if (SelectedItem == null)
            return;

        if (!string.IsNullOrWhiteSpace(DisplayMemberPath))
        {
            var displayValue =
                GetPropertyValue(
                    SelectedItem,
                    DisplayMemberPath);

            SearchText =
                displayValue?.ToString() ?? string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(SelectedValuePath))
        {
            SelectedValue =
                GetPropertyValue(
                    SelectedItem,
                    SelectedValuePath);
        }
    }


    // ============================================================
    // Reflection helper
    // ============================================================

    private static object? GetPropertyValue(
        object obj,
        string propertyPath)
    {
        if (obj == null ||
            string.IsNullOrWhiteSpace(propertyPath))
        {
            return null;
        }

        object? current = obj;

        foreach (var propertyName in propertyPath.Split('.'))
        {
            if (current == null)
                return null;

            var property =
                TypeDescriptor
                    .GetProperties(current)
                    .Find(propertyName, true);

            if (property == null)
                return null;

            current = property.GetValue(current);
        }

        return current;
    }


    // ============================================================
    // TextBox Focus
    // ============================================================


    private void TextBox_GotFocus(
        object sender,
        RoutedEventArgs e)
    {
        if (!IsEnabled)
            return;

        IsDropDownOpen = true;

        _view?.Refresh();
    }




    // ============================================================
    // Keyboard
    // ============================================================

    private void TextBox_PreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        switch (e.Key)
        {
            // ----------------------------------------------------
            // ↓
            // ----------------------------------------------------

            case Key.Down:

                IsDropDownOpen = true;

                MoveSelection(1);

                e.Handled = true;

                break;


            // ----------------------------------------------------
            // ↑
            // ----------------------------------------------------

            case Key.Up:

                IsDropDownOpen = true;

                MoveSelection(-1);

                e.Handled = true;

                break;


            // ----------------------------------------------------
            // Enter
            // ----------------------------------------------------

            case Key.Enter:

                SelectCurrentRow();

                e.Handled = true;

                break;


            // ----------------------------------------------------
            // Escape
            // ----------------------------------------------------

            case Key.Escape:

                RestorePreviousSelection();

                IsDropDownOpen = false;

                e.Handled = true;

                break;
        }
    }


    // ============================================================
    // Move DataGrid selection
    // ============================================================

    private void MoveSelection(int direction)
    {
        if (!IsDropDownOpen)
        {
            IsDropDownOpen = true;
        }

        if (PART_DataGrid.Items.Count == 0)
            return;

        var currentIndex =
            PART_DataGrid.SelectedIndex;

        var newIndex = currentIndex;

        if (currentIndex < 0)
        {
            newIndex =
                direction > 0
                    ? 0
                    : PART_DataGrid.Items.Count - 1;
        }
        else
        {
            newIndex =
                currentIndex + direction;
        }

        if (newIndex < 0)
            newIndex = 0;

        if (newIndex >= PART_DataGrid.Items.Count)
            newIndex = PART_DataGrid.Items.Count - 1;

        PART_DataGrid.SelectedIndex = newIndex;

        PART_DataGrid.ScrollIntoView(
            PART_DataGrid.SelectedItem);
    }


    // ============================================================
    // Enter
    // ============================================================

    private void SelectCurrentRow()
    {
        if (PART_DataGrid.SelectedItem == null)
            return;

        SelectItem(PART_DataGrid.SelectedItem);
    }


    // ============================================================
    // Escape
    // ============================================================

    private void RestorePreviousSelection()
    {
        if (_previousSelectedItem != null)
        {
            _isSelectingItem = true;

            try
            {
                SelectedItem = _previousSelectedItem;

                PART_DataGrid.SelectedItem =
                    _previousSelectedItem;

                UpdateDisplayFromSelectedItem();
            }
            finally
            {
                _isSelectingItem = false;
            }
        }
        else
        {
            SearchText = string.Empty;
        }
    }
}