using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.Common;
using ITAM.WPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ITAM.WPF.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        public virtual string Title => string.Empty;
        public ToolbarContext ToolbarContext { get; set; }
        protected readonly INavigationService _navigationService;

        // navigationService : dùng để close View
        // ToolbarContext : dùng để bind các command của toolbar (bao gồm cả điều kiện canExecute)
        public BaseViewModel(INavigationService navigationService)
        {
            InitToolbarState();
            ToolbarContext = GetToolbarContext();
            _navigationService = navigationService;
        }

        // Tạo một ToolbarContext dựa trên các command của ViewModel hiện tại
        public virtual ToolbarContext GetToolbarContext()
        {
            ToolbarContext toolbarContext = new ToolbarContext
            {
                AddCommand = AddCommand,
                EditCommand = EditCommand,
                DeleteCommand = DeleteCommand,
                SaveCommand = SaveCommand,
                CancelCommand = CancelCommand,
                CloseCommand = CloseCommand,
                RefreshCommand=RefreshCommand
            };
            return toolbarContext;
        }
        // Khởi tạo trạng thái của toolbar (các command có thể thực hiện hay không)
        protected virtual void InitToolbarState() { }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        protected bool canAdd;

        [RelayCommand(CanExecute = nameof(CanAdd))]
        protected virtual void Add()
        {
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditCommand))]
        protected bool canEdit;

        [RelayCommand(CanExecute = nameof(CanEdit))]
        protected virtual void Edit()
        {
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
        protected bool canDelete;

        [RelayCommand(CanExecute = nameof(CanDelete))]
        protected virtual void Delete()
        {
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        protected bool canSave;

        [RelayCommand(CanExecute = nameof(CanSave))]
        protected virtual void Save()
        {
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
        protected bool canCancel;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        protected virtual void Cancel()
        {
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CloseCommand))]
        protected bool canClose;

        [RelayCommand(CanExecute = nameof(CanClose))]
        protected virtual void Close()
        {
            _navigationService.CloseView(this);
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
        protected bool canRefresh;

        [RelayCommand(CanExecute = nameof(CanRefresh))]
        protected virtual void Refresh()
        {
           
        }
    }

}
