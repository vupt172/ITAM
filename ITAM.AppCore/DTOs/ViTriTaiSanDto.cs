using CommunityToolkit.Mvvm.ComponentModel;
using Mapster;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.AppCore.DTOs
{
    public partial class ViTriTaiSanDto:ObservableValidator
    {
        [ObservableProperty]
        private long id;

        [ObservableProperty]
        [Required(ErrorMessage = "Mã không được để trống.")]
        private string code = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Tên không được để trống.")]
        private string name = string.Empty;

        [ObservableProperty]
        private string? description;

        [ObservableProperty]
        private bool isActive = true;

        // Khóa ngoại - dùng khi tạo/sửa (bind vào ComboBox chọn Phòng ban)
        [ObservableProperty]
        private long phongBanId;

        // Thông tin hiển thị - dùng khi đọc/liệt kê (tránh phải load navigation ở View)
        [ObservableProperty]
        private string? phongBanName;

        public bool Validate()
        {
            ValidateAllProperties();
            return !HasErrors;
        }
        public ViTriTaiSanDto Clone()
        {
            return this.Adapt<ViTriTaiSanDto>();
        }
    }
}
