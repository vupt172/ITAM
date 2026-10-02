using CommunityToolkit.Mvvm.Input;
using ITAM.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ITAM.ViewModels
{
    public class OpenedWindowItem
    {
        public OpenedWindowItem(int index, BaseViewModel viewModel, bool isActive, Action<BaseViewModel> activate)
        {
            ViewModel = viewModel;
            IsActive = isActive;
            // "_" tạo phím tắt gạch chân như ảnh (chỉ hợp lệ với 1-9)
            DisplayText = (index <= 9 ? $"_{index} " : $"{index} ") + viewModel.Title;
            ActivateCommand = new RelayCommand(() => activate(viewModel));
        }

        public BaseViewModel ViewModel { get; }
        public string DisplayText { get; }
        public bool IsActive { get; }
        public ICommand ActivateCommand { get; }
    }
}
