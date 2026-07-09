using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.AppCore.Interfaces
{
    public interface INavigationService
    {
        object? CurrentView { get; }
        string CurrentTitle { get; set; }

        void NavigateTo<TViewModel>(string? title = null) where TViewModel : class;
        void NavigateTo(object viewModel, string? title =null);

        void GoBack();
        bool CanGoBack { get; }
    }
}
