using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Exceptions;
using ITAM.Domain.Interfaces;
using ITAM.WPF.Services.Interfaces;
using Mapster;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ITAM.WPF.ViewModels.Catalogs
{
    public partial class HangHoaViewModel : CatalogViewModel
    {
        private readonly IHangHoaService _hangHoaService;
        private readonly ICatalogService<DMTaiSan> _dmTaiSanService;
        private readonly IFileStorageService _fileStorageService;
        public ObservableCollection<HangHoaDto> Items { get; } = [];
        public ObservableCollection<DMTaiSanDto> DsDMTaiSan { get; } = [];
        public ICollectionView ItemsView { get; }

        [ObservableProperty] private HangHoaDto? selectedItem;
        [ObservableProperty] private HangHoaDto currentItem = new();
        [ObservableProperty] private string? searchText;

        protected override bool HasSelection => SelectedItem != null;
        [ObservableProperty]
        private ImageSource? imageSource; //ảnh đang preview trên UI

        private string? _oldImagePath; //ảnh cũ trong DB

        private string? _pendingImageFile; //file người dùng vừa chọn, chưa upload

        private string? _pendingImageExtension; //.jpg / .png / .webp

        public HangHoaViewModel(
            IHangHoaService hangHoaService,
            ICatalogService<DMTaiSan> dmTaiSanService,
            IFileStorageService fileStorageService,
            IErrorDialogService errorDialogService) : base(errorDialogService)
        {
            _hangHoaService = hangHoaService;
            _dmTaiSanService = dmTaiSanService;
            _fileStorageService = fileStorageService;
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
            ImageSource = null;

            _oldImagePath = null;
            _pendingImageFile = null;
            _pendingImageExtension = null;
        }

        protected override void Edit()
        {
            base.Edit();
            CurrentItem = SelectedItem!.Clone();
            LoadCurrentImage();
        }

        protected override async Task Delete()
        {
            try
            {
                var item = SelectedItem!;

                await _hangHoaService.DeleteAsync(item.Id);

                if (!string.IsNullOrWhiteSpace(item.ImagePath))
                {
                    await _fileStorageService.DeleteAsync(item.ImagePath);
                }

                await LoadAsync();

                SelectedItem = null;
                CurrentItem = new HangHoaDto();

                ImageSource = null;
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
        }

        protected override async Task Save()
        {
            if (!CurrentItem.Validate())
                return;

            string? newImagePath = null;

            try
            {
                var oldImagePath = CurrentItem.ImagePath;

                // 1. Nếu người dùng chọn ảnh mới
                if (!string.IsNullOrWhiteSpace(_pendingImageFile))
                {
                    await using var stream =
                        File.OpenRead(_pendingImageFile);

                    newImagePath = await _fileStorageService.SaveAsync(
                        stream,
                        "Images/HangHoa",
                        _pendingImageExtension!);

                    CurrentItem.ImagePath = newImagePath;
                }

                // 2. Map DTO -> Entity
                var entity = CurrentItem.Adapt<HangHoa>();

                // 3. Lưu DB
                if (entity.Id == 0)
                    await _hangHoaService.CreateAsync(entity);
                else
                    await _hangHoaService.UpdateAsync(entity.Id, entity);

                // 4. DB thành công -> xóa ảnh cũ nếu đã thay ảnh
                if (!string.IsNullOrWhiteSpace(newImagePath)
                    && !string.IsNullOrWhiteSpace(oldImagePath)
                    && !string.Equals(
                        oldImagePath,
                        newImagePath,
                        StringComparison.OrdinalIgnoreCase))
                {
                    await _fileStorageService.DeleteAsync(oldImagePath);
                }

                // 5. Reload
                await LoadAsync();

                SelectedItem = Items.FirstOrDefault(
                    x => x.Id == entity.Id);

                CurrentItem =
                    SelectedItem?.Clone()
                    ?? new HangHoaDto();

                LoadCurrentImage();

                _pendingImageFile = null;
                _pendingImageExtension = null;
                _oldImagePath = CurrentItem.ImagePath;

                await base.Save();
            }
            catch (InvalidBusinessRuleException ex)
            {
                MessageBox.Show(ex.Message);

                // Nếu upload thành công nhưng DB thất bại
                // thì xóa file mới để tránh file rác.
                if (!string.IsNullOrWhiteSpace(newImagePath))
                {
                    try
                    {
                        await _fileStorageService.DeleteAsync(newImagePath);
                    }
                    catch
                    {
                        // Không che mất lỗi nghiệp vụ ban đầu.
                    }
                }
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);

                if (!string.IsNullOrWhiteSpace(newImagePath))
                {
                    try
                    {
                        await _fileStorageService.DeleteAsync(newImagePath);
                    }
                    catch
                    {
                    }
                }
            }
        }

        protected override void Cancel()
        {
            base.Cancel();

            CurrentItem =
                SelectedItem?.Clone()
                ?? new HangHoaDto();

            LoadCurrentImage();

            _pendingImageFile = null;
            _pendingImageExtension = null;
        }

        partial void OnSelectedItemChanged(HangHoaDto? value)
        {
            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
            CurrentItem = value?.Clone() ?? new HangHoaDto();

            LoadCurrentImage();
        }
        private void LoadCurrentImage()
        {
            _pendingImageFile = null;
            _pendingImageExtension = null;

            _oldImagePath = CurrentItem.ImagePath;

            if (string.IsNullOrWhiteSpace(CurrentItem.ImagePath))
            {
                ImageSource = null;
                return;
            }

            try
            {
                var fullPath =
                    _fileStorageService.GetFullPath(CurrentItem.ImagePath);

                if (!File.Exists(fullPath))
                {
                    ImageSource = null;
                    return;
                }

                ImageSource = LoadImage(fullPath);
            }
            catch
            {
                ImageSource = null;
            }
        }
        [RelayCommand]
        private void SelectImage()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Chọn hình ảnh hàng hóa",
                Filter = "Hình ảnh|*.jpg;*.jpeg;*.png;*.webp",
                Multiselect = false
            };

            if (dialog.ShowDialog() != true)
                return;

            var extension = Path.GetExtension(dialog.FileName);

            _pendingImageFile = dialog.FileName;
            _pendingImageExtension = extension;

            ImageSource = LoadImage(dialog.FileName);
        }
        private static ImageSource LoadImage(string filePath)
        {
            var image = new BitmapImage();

            using var stream = File.OpenRead(filePath);

            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();

            image.Freeze();

            return image;
        }
        [RelayCommand]
        private void RemoveImage()
        {
            ImageSource = null;

            _pendingImageFile = null;
            _pendingImageExtension = null;

            CurrentItem.ImagePath = null;
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
