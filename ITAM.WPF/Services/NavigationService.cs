using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.AppCore.Interfaces;
using ITAM.WPF.Constants;
using ITAM.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ITAM.WPF.Services
{   /// <summary>
    /// NavigationService là dịch vụ điều hướng giữa các ViewModel trong ứng dụng WPF. Nó quản lý ngăn xếp điều hướng và cung cấp các phương thức để điều hướng đến các ViewModel khác nhau.
    /// </summary>
    public partial class NavigationService : ObservableObject, INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ToolbarViewModel _toolbarViewModel;

        private readonly Dictionary<Type, BaseViewModel> _openedViews = new();
        public ObservableCollection<BaseViewModel> OpenedViews { get; }
    = new();
        [ObservableProperty]
        private BaseViewModel? currentView;
        [ObservableProperty]
        private string currentTitle = PageTitles.Default;

        public NavigationService(IServiceProvider serviceProvider, ToolbarViewModel toolbarViewModel)
        {
            _serviceProvider = serviceProvider;
            _toolbarViewModel = toolbarViewModel;
        }
        public void NavigateTo<TViewModel>() where TViewModel : BaseViewModel
        {
            BaseViewModel vm;

            if (_openedViews.TryGetValue(typeof(TViewModel), out vm))
            {
                OpenedViews.Remove(vm);
                OpenedViews.Add(vm);
            }
            else
            {
                vm = _serviceProvider.GetRequiredService<TViewModel>();
                _openedViews.Add(typeof(TViewModel), vm);
                OpenedViews.Add(vm);
            }
            ActivateView(vm);
        }
        // Kích hoạt View ra màn hình chính
        public void ActivateView(BaseViewModel viewModel)
        {
            CurrentView = viewModel;
            UpdateToolbar(viewModel);
            CurrentTitle = viewModel.Title;
        }

        private void UpdateToolbar(BaseViewModel viewModel)
        {
            if (viewModel is IToolbarAware toolbarAware)
            {
                _toolbarViewModel.Apply(toolbarAware.ToolbarState);
            }
            else
            {
                _toolbarViewModel.Default();
            }
        }

        public void CloseView(BaseViewModel viewModel)
        {
            var viewType = viewModel.GetType();

            _openedViews.Remove(viewType);
            OpenedViews.Remove(viewModel);

            if (CurrentView == viewModel)
            {
                if (OpenedViews.Count > 0)
                {
                    ActivateView(OpenedViews[^1]);
                }
                else
                {
                    CurrentView = null;
                    CurrentTitle = PageTitles.Default;
                    _toolbarViewModel.Default();
                }
            }
        }
    }

}