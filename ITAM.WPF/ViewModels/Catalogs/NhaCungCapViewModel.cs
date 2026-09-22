using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.AppCore.DTOs;
using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Exceptions;
using ITAM.Domain.Interfaces;
using ITAM.WPF.Services.Interfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ITAM.WPF.ViewModels.Catalogs
{
    public partial class NhaCungCapViewModel : CatalogViewModel
    {
        #region Fields & Properties
        private readonly ICatalogService<NhaCungCap> _nhaCungCapService;
        public ICollectionView ItemsView { get; }
        [ObservableProperty]
        private string? searchText;
        public ObservableCollection<NhaCungCapDto> Items { get; } = [];
        [ObservableProperty]
        private NhaCungCapDto? selectedItem; // Bản ghi đang được chọn trong DataGrid
        [ObservableProperty]
        private NhaCungCapDto currentItem = new(); // Bản ghi đang hiển thị trên form để chỉnh sửa hoặc thêm mới
        protected override bool HasSelection => SelectedItem != null;
        #endregion
        #region Contructor & Initialization
        public NhaCungCapViewModel(ICatalogService<NhaCungCap> nhaCungCapservice, IErrorDialogService errorDialogService) : base(errorDialogService)
        {
            _nhaCungCapService = nhaCungCapservice;
            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsView.Filter = FilterData;
            _ = LoadAsync();

        }
        public override async Task LoadAsync()
        {
            Items.Clear();
            var entities = await _nhaCungCapService.GetAllAsync(true);
            foreach (var dto in entities.Adapt<List<NhaCungCapDto>>())
            {
                Items.Add(dto);
            }
        }

        #endregion
        #region Commands

        protected override void Add()
        {
            base.Add();
            CurrentItem = new NhaCungCapDto();
        }


        protected override void Edit()
        {
            base.Edit();
            CurrentItem = SelectedItem!.Clone();
        }
        protected override async Task Delete()
        {
            try
            {
                await _nhaCungCapService.DeleteAsync(SelectedItem!.Id);
                await LoadAsync();
                SelectedItem = null;
                CurrentItem = new NhaCungCapDto();
            }
            catch (Exception e)
            {
                _errorDialogService.Show(e);
            }

        }

        protected override async Task Save()
        {
            // validate form
            if (!CurrentItem.Validate()) return;
            try
            {
                var entity = CurrentItem.Adapt<NhaCungCap>();
                if (entity.Id == 0)
                    await _nhaCungCapService.CreateAsync(entity);
                else
                    await _nhaCungCapService.UpdateAsync(entity.Id, entity);

                // Load lại danh sách bản ghi cho DataGrid và chọn bản ghi vừa được thêm hoặc cập nhật
                await LoadAsync();
                SelectedItem = Items.FirstOrDefault(x => x.Id == entity.Id);
                CurrentItem = SelectedItem?.Clone() ?? new();
                await base.Save();
            }
            catch (InvalidBusinessRuleException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
        }

        protected override void Cancel()
        {
            base.Cancel();
            CurrentItem = SelectedItem?.Clone() ?? new NhaCungCapDto();
        }
        #endregion
        #region INotifyPropertyChanged Implementation
        partial void OnSelectedItemChanged(NhaCungCapDto? value)
        {
            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
            CurrentItem = value;
        }
        partial void OnSearchTextChanged(string? value)
        {
            ItemsView.Refresh();
        }
        #endregion
        #region Helpers
        private bool FilterData(object obj)
        {
            if (obj is not NhaCungCapDto item)
                return false;

            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            return
                item.Code.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                || item.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }
}
