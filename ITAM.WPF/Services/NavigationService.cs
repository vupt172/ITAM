using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.AppCore.Interfaces;
using ITAM.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace ITAM.WPF.Services
{   /// <summary>
    /// NavigationService là dịch vụ điều hướng giữa các ViewModel trong ứng dụng WPF. Nó quản lý ngăn xếp điều hướng và cung cấp các phương thức để điều hướng đến các ViewModel khác nhau.
    /// </summary>
    public partial class NavigationService : ObservableObject, INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Stack<object> _backStack = new();

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [ObservableProperty]
        private object? currentView;

        [ObservableProperty]
        private string currentTitle = string.Empty;


        public bool CanGoBack => _backStack.Count > 0;

        public void NavigateTo<TViewModel>(string? title = null)
            where TViewModel : class
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

            NavigateTo(viewModel, title);
        }  

        public void NavigateTo(object viewModel, string? title = null)
        {
            if (CurrentView != null)
                _backStack.Push(CurrentView);

            CurrentView = viewModel;

            // ưu tiên title truyền vào, không thì lấy từ BaseViewModel
            CurrentTitle = title ?? (viewModel as BaseViewModel)?.Title ?? "";
            OnPropertyChanged(nameof(CanGoBack));
        }

        public void GoBack()
        {
            if (_backStack.Count == 0) return;

            CurrentView = _backStack.Pop();

            CurrentTitle = (CurrentView as BaseViewModel)?.Title ?? "";

            OnPropertyChanged(nameof(CanGoBack));
        }
    }

}