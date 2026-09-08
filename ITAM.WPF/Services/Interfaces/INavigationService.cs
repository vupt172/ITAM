using ITAM.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.WPF.Services.Interfaces
{
    public interface INavigationService
    {
        BaseViewModel? CurrentView { get; }
        string CurrentTitle { get; }
        ObservableCollection<BaseViewModel> OpenedViews { get; }
        void NavigateTo<TViewModel>() where TViewModel : BaseViewModel;
        void ActivateView(BaseViewModel viewModel);
        void CloseView(BaseViewModel viewModel);
        void Reset();   // ⬅ thêm mới
    }
}
