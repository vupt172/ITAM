using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.AppCore.DTOs;
using ITAM.Domain.Entities;
using ITAM.Domain.Interfaces;
using ITAM.WPF.ViewModels;
using Mapster;
using System.Collections.ObjectModel;
namespace ITAM.WPF.ViewModels.Catalogs
{
    public partial class PhongBanViewModel : CatalogViewModel
    {
        #region Fields & Properties
        private readonly ICatalogService<PhongBan> _phongBanService;
        public ObservableCollection<PhongBanDto> Items { get; } = [];

        [ObservableProperty]
        private PhongBanDto? selectedItem; // Bản ghi đang được chọn trong DataGrid

        [ObservableProperty]
        private PhongBanDto currentItem = new(); // Bản ghi đang hiển thị trên form để chỉnh sửa hoặc thêm mới

        protected override bool HasSelection => SelectedItem != null;
        #endregion

        #region Contructor & Initialization
        public PhongBanViewModel(ICatalogService<PhongBan> phongBanService)
        {
            _phongBanService = phongBanService;
            _ = LoadAsync();
        }
        public override async Task LoadAsync()
        {
            var entities = await _phongBanService.GetAllActiveAsync(true);
            Items.Clear();
            foreach (var dto in entities.Adapt<List<PhongBanDto>>())
            {
                Items.Add(dto);
            }
        }
        #endregion


        #region Commands
        protected override void Add()
        {
            base.Add();

            CurrentItem = new PhongBanDto();
        }

        protected override void Edit()
        {
            base.Edit();

            CurrentItem = SelectedItem!.Clone();
        }

        protected override async Task Save()
        {
            var entity = CurrentItem.Adapt<PhongBan>();
            if (entity.Id == 0)
                await _phongBanService.CreateAsync(entity);
            else
                await _phongBanService.UpdateAsync(entity.Id, entity);

            // Load lại danh sách bản ghi cho DataGrid và chọn bản ghi vừa được thêm hoặc cập nhật
            await LoadAsync();
            SelectedItem = Items.FirstOrDefault(x => x.Id == entity.Id);
            CurrentItem = SelectedItem?.Clone() ?? new();
            await base.Save();
        }

        protected override void Cancel()
        {
            base.Cancel();

            CurrentItem = SelectedItem?.Clone() ?? new PhongBanDto();
        }
        #endregion
    }
}