using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.Common;
using ITAM.WPF.Services.Interfaces;

namespace ITAM.WPF.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        public virtual string Title => string.Empty;
        public ToolbarContext ToolbarContext { get; set; }
        protected readonly INavigationService _navigationService;
        // Cờ chỉnh sửa chung — Add/Edit bật lên, Save/Cancel tắt đi.
        [ObservableProperty]
        private bool isEditing;

        // Lớp con override để cho biết đang có dòng được chọn hay không
        // (áp dụng cho màn có DataGrid: User/Role/ThamSoNguoiDung...)
        protected virtual bool HasSelection => false;
        public bool IsReadOnly => !IsEditing;

        public BaseViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            InitToolbarState();
            ToolbarContext = GetToolbarContext();
        }

        public virtual ToolbarContext GetToolbarContext()
        {
            return new ToolbarContext
            {
                AddCommand = AddCommand,
                EditCommand = EditCommand,
                DeleteCommand = DeleteCommand,
                SaveCommand = SaveCommand,
                CancelCommand = CancelCommand,
                CloseCommand = CloseCommand,
                RefreshCommand = RefreshCommand
            };
        }

        // Dùng để set canClose/canRefresh (2 cờ không phụ thuộc IsEditing).
        // Add/Edit/Delete/Save/Cancel giờ tự suy ra từ IsEditing + HasSelection,
        // không cần gán tay trong hàm này nữa.
        protected virtual void InitToolbarState() { }

   

        [RelayCommand(CanExecute = nameof(CanAdd))]
        protected virtual void Add() => IsEditing = true;
        protected virtual bool CanAdd() => !IsEditing;

        [RelayCommand(CanExecute = nameof(CanEdit))]
        protected virtual void Edit() => IsEditing = true;
        protected virtual bool CanEdit() => !IsEditing && HasSelection;

        [RelayCommand(CanExecute = nameof(CanDelete))]
        protected virtual void Delete() { }
        protected virtual bool CanDelete() => !IsEditing && HasSelection;

        [RelayCommand(CanExecute = nameof(CanSave))]
        protected virtual void Save() => IsEditing = false;
        protected virtual bool CanSave() => IsEditing;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        protected virtual void Cancel() => IsEditing = false;
        protected virtual bool CanCancel() => IsEditing;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CloseCommand))]
        protected bool canClose;
        [RelayCommand(CanExecute = nameof(CanClose))]
        protected virtual void Close() => _navigationService.CloseView(this);
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
        protected bool canRefresh;
        [RelayCommand(CanExecute = nameof(CanRefresh))]
        protected virtual void Refresh() { }

        partial void OnIsEditingChanged(bool value)
        {
            OnPropertyChanged(nameof(IsReadOnly));
            AddCommand.NotifyCanExecuteChanged();
            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
            SaveCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
        }

        // Lớp con gọi hàm này trong OnSelectedXxxChanged để cập nhật lại
        // trạng thái Edit/Delete khi người dùng chọn dòng khác trên DataGrid.
        protected void NotifySelectionChanged()
        {
            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }
    }
}