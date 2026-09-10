using CommunityToolkit.Mvvm.ComponentModel;
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

        [Range(1, long.MaxValue, ErrorMessage = "Phải chọn danh mục tài sản.")]
        public long DMTaiSanId { get; set; }

        public string? DMTaiSanName { get; set; }
        [ObservableProperty] private string? description;
        [ObservableProperty] private bool isActive = true;

        public bool Validate()
        {
            ValidateAllProperties();
            return !HasErrors;
        }

        public HangHoaDto Clone() => new()
        {
            Id = Id, Code = Code, Name = Name, HangSanXuat = HangSanXuat,
            Model = Model, DMTaiSanId = DMTaiSanId, DMTaiSanName = DMTaiSanName,
            Description = Description, IsActive = IsActive
        };
    }
}
