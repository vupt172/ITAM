using CommunityToolkit.Mvvm.ComponentModel;
using Mapster;
using System.ComponentModel.DataAnnotations;

namespace ITAM.AppCore.DTOs
{
    public partial class HangHoaDto : ObservableValidator
    {
        [ObservableProperty] private long id;

        [ObservableProperty]
        [Required(ErrorMessage = "Mã hàng hóa không được để trống.")]
        private string code = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Tên hàng hóa không được để trống.")]
        private string name = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Hãng sản xuất không được để trống.")]
        private string hangSanXuat = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Model không được để trống.")]
        private string model = string.Empty;
        [ObservableProperty]
        [Required(ErrorMessage = "Đơn vị tính không được để trống.")]
        private string donViTinh = string.Empty;
        [ObservableProperty]
        private bool requireSerial;
        [Range(1, long.MaxValue, ErrorMessage = "Phải chọn danh mục tài sản.")]
        [ObservableProperty]
        private string? imagePath = string.Empty;
        public long DMTaiSanId { get; set; }
        public string? DMTaiSanName { get; set; }
        [ObservableProperty] private string? description;
        [ObservableProperty] private bool isActive = true;

        public bool Validate()
        {
            ValidateAllProperties();
            return !HasErrors;
        }

        public HangHoaDto Clone() 
        {
           return this.Adapt<HangHoaDto>();
        }
    }
}
