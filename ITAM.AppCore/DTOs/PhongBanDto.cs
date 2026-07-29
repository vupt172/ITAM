using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.AppCore.DTOs
{
    public partial class PhongBanDto : ObservableValidator
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        [Required(ErrorMessage = "Mã phòng ban không được để trống.")]
        private string name = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Tên phòng ban không được để trống.")]
        private string code = string.Empty;

        [ObservableProperty]
        private string? description;

        [ObservableProperty]
        private bool isActive = true;

        public PhongBanDto Clone()
        {
            return new PhongBanDto
            {
                Id = Id,
                Code = Code,
                Name = Name,
                Description = Description,
                IsActive = IsActive
            };
        }
    }
}
