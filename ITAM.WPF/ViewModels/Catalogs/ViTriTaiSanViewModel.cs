using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs;
using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Exceptions;
using ITAM.Domain.Interfaces;
using ITAM.WPF.Services.Interfaces;
using ITAM.WPF.ViewModels;
using Mapster;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
namespace ITAM.WPF.ViewModels.Catalogs
{
    public partial class ViTriTaiSanViewModel : CatalogViewModel
    {
        #region Fields & Properties
        private readonly IViTriTaiSanService _viTriTaiSanService;
        private readonly ICatalogService<PhongBan> _phongBanService;


        public ObservableCollection<ViTriTaiSanDto> Items { get; } = [];
        // Danh sách nguồn cho ComboBox - load 1 lần, không đổi liên tục theo CurrentItem
        [ObservableProperty]
        private ObservableCollection<PhongBanDto> _dsPhongBan = new();

        [ObservableProperty]
        private ViTriTaiSanDto? selectedItem; // Bản ghi đang được chọn trong DataGrid
        [ObservableProperty]
        private ViTriTaiSanDto currentItem = new(); // Bản ghi đang hiển thị trên form để chỉnh sửa hoặc thêm mới
        protected override bool HasSelection => SelectedItem != null;
        public ICollectionView ItemsView { get; }
        [ObservableProperty]
        private string? searchText;
        #endregion
        #region Contructor & Initialization
        public ViTriTaiSanViewModel(IViTriTaiSanService viTriTaiSanService, ICatalogService<PhongBan> phongBanService, IErrorDialogService errorDialogService) : base(errorDialogService)
        {
            _viTriTaiSanService = viTriTaiSanService;
            _phongBanService = phongBanService;
            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsView.Filter = FilterData;
            _ = LoadAsync();

        }
        public override async Task LoadAsync()
        {
            var dsPhongBan = await _phongBanService.GetAllAsync();
            DsPhongBan = dsPhongBan.Adapt<ObservableCollection<PhongBanDto>>();
            Items.Clear();
            var entities = await _viTriTaiSanService.GetAllAsync(true);
            foreach (var dto in entities.Adapt<List<ViTriTaiSanDto>>())
            {
                Items.Add(dto);
            }
            //Items = dsViTriTaiSan.Adapt<ObservableCollection<ViTriTaiSanDto>>();

        }

        #endregion
        #region Commands

        protected override void Add()
        {
            base.Add();
            CurrentItem = new ViTriTaiSanDto();
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
                await _viTriTaiSanService.DeleteAsync(SelectedItem!.Id);
                await LoadAsync();
                SelectedItem = null;
                CurrentItem = new ViTriTaiSanDto();
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
                var entity = CurrentItem.Adapt<ViTriTaiSan>();
                if (entity.Id == 0)
                    await _viTriTaiSanService.CreateAsync(entity);
                else
                    await _viTriTaiSanService.UpdateAsync(entity.Id, entity);

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
            CurrentItem = SelectedItem?.Clone() ?? new ViTriTaiSanDto();
        }
        #endregion
        #region INotifyPropertyChanged Implementation
        partial void OnSelectedItemChanged(ViTriTaiSanDto? value)
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
            if (obj is not ViTriTaiSanDto item)
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