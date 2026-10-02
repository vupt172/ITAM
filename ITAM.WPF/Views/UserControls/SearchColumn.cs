
using System.Windows;
using System.Windows.Controls;

namespace ITAM.WPF.Views.UserControls;

public class SearchColumn : DependencyObject
{
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(SearchColumn));

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }


    public static readonly DependencyProperty BindingProperty =
        DependencyProperty.Register(
            nameof(Binding),
            typeof(string),
            typeof(SearchColumn));

    public string Binding
    {
        get => (string)GetValue(BindingProperty);
        set => SetValue(BindingProperty, value);
    }


    public static readonly DependencyProperty WidthProperty =
        DependencyProperty.Register(
            nameof(Width),
            typeof(DataGridLength),
            typeof(SearchColumn),
            new PropertyMetadata(new DataGridLength(1, DataGridLengthUnitType.Star)));

    public DataGridLength Width
    {
        get => (DataGridLength)GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }
}

