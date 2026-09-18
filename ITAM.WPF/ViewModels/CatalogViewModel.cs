using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.Domain.Enums;
using ITAM.WPF.Services.Interfaces;

namespace ITAM.WPF.ViewModels
{
    public abstract partial class CatalogViewModel : ObservableObject
    {
        protected readonly IErrorDialogService _errorDialogService;
 
        public CatalogViewModel(IErrorDialogService errorDialogService)
        {
            _errorDialogService = errorDialogService;

        }
        #region Fields & Properties
        [ObservableProperty]
        private bool isEditing;
        public bool IsReadOnly => !IsEditing;
        protected abstract bool HasSelection { get; }   // Lớp kế thừa trả về true nếu đang có bản ghi được chọn.
        #endregion
        #region Constructor & Initialization
        public abstract Task LoadAsync();
        #endregion
        #region Commands

        [RelayCommand(CanExecute = nameof(CanAdd))]
        protected virtual void Add()
        {
            IsEditing = true;
        }

        private bool CanAdd() => !IsEditing;

        [RelayCommand(CanExecute = nameof(CanEdit))]
        protected virtual void Edit()
        {
            IsEditing = true;
        }

        private bool CanEdit() => !IsEditing && HasSelection;

        [RelayCommand(CanExecute = nameof(CanSave))]
        protected virtual async Task Save()
        {
            IsEditing = false;
            await Task.CompletedTask;
        }

        private bool CanSave() => IsEditing;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        protected virtual void Cancel()
        {
            IsEditing = false;
        }

        private bool CanCancel() => IsEditing;

        [RelayCommand(CanExecute = nameof(CanDelete))]
        protected virtual async Task Delete()
        {
            await Task.CompletedTask;
        }

        private bool CanDelete() => !IsEditing && HasSelection;

        #endregion
        #region INotifyPropertyChanged Implementation
        partial void OnIsEditingChanged(bool value)
        {
            OnPropertyChanged(nameof(IsReadOnly));

            AddCommand.NotifyCanExecuteChanged();
            EditCommand.NotifyCanExecuteChanged();
            SaveCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }
        #endregion
   
    }
}