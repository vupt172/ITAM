using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.AppCore.DTOs;
using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Exceptions;
using ITAM.Domain.Interfaces;
using ITAM.WPF.Services.Interfaces;
using Mapster;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace ITAM.WPF.ViewModels.Catalogs
{
    public partial class HangHoaViewModel : CatalogViewModel
    {
        private readonly IHangHoaService _hangHoaService;
        private readonly ICatalogService<DMTaiSan> _dmTaiSanService;

        public ObservableCollection<HangHoaDto> Items { get; } = [];
        public ObservableCollection<DMTaiSanDto> DsDMTaiSan { get; } = [];
        public ICollectionView ItemsView { get; }

        [ObservableProperty] private HangHoaDto? selectedItem;
        [ObservableProperty] private HangHoaDto currentItem = new();
        [ObservableProperty] private string? searchText;

        protected override bool HasSelection => SelectedItem != null;

        public HangHoaViewModel(
            IHangHoaService hangHoaService,
            ICatalogService<DMTaiSan> dmTaiSanService,
            IErrorDialogService errorDialogService) : base(errorDialogService)
        {
            _hangHoaService = hangHoaService;
            _dmTaiSanService = dmTaiSanService;
            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsView.Filter = FilterData;
            _ = LoadAsync();
        }

        public override async Task LoadAsync()
        {
            DsDMTaiSan.Clear();
            foreach (var item in (await _dmTaiSanService.GetAllAsync()).Adapt<List<DMTaiSanDto>>())
                DsDMTaiSan.Add(item);

            Items.Clear();
            foreach (var item in (await _hangHoaService.GetAllWithDanhMucAsync(true)).Adapt<List<HangHoaDto>>())
                Items.Add(item);
        }

        protected override void Add()
        {
            base.Add();
            CurrentItem = new HangHoaDto();
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
                await _hangHoaService.DeleteAsync(SelectedItem!.Id);
                await LoadAsync();
                SelectedItem = null;
                CurrentItem = new HangHoaDto();
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
        }

        protected override async Task Save()
        {
            if (!CurrentItem.Validate()) return;

            try
            {
                var entity = CurrentItem.Adapt<HangHoa>();
                if (entity.Id == 0)
                    await _hangHoaService.CreateAsync(entity);
                else
                    await _hangHoaService.UpdateAsync(entity.Id, entity);

                await LoadAsync();
                SelectedItem = Items.FirstOrDefault(x => x.Id == entity.Id);
                CurrentItem = SelectedItem?.Clone() ?? new HangHoaDto();
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
            CurrentItem = SelectedItem?.Clone() ?? new HangHoaDto();
        }

        partial void OnSelectedItemChanged(HangHoaDto? value)
        {
            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
            CurrentItem = value?.Clone() ?? new HangHoaDto();
        }

        partial void OnSearchTextChanged(string? value) => ItemsView.Refresh();

        private bool FilterData(object obj)
        {
            if (obj is not HangHoaDto item || string.IsNullOrWhiteSpace(SearchText))
                return obj is HangHoaDto;

            return item.Code.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                || item.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                || item.HangSanXuat.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                || item.Model.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        }
    }
}
