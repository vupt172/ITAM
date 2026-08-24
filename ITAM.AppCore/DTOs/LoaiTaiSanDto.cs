using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.AppCore.DTOs
{
    public partial class LoaiTaiSanDto : ObservableValidator
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

        [ObservableProperty]
        [Range(typeof(decimal), "0", "999999999")]
        private decimal? minValue;
        [Range(typeof(decimal), "0", "999999999")]
        [ObservableProperty]
        private decimal? maxValue;
        [ObservableProperty]
        private int displayOrder = 0;

        public bool Validate()
        {
            ValidateAllProperties();
            return !HasErrors;
        }
        public LoaiTaiSanDto Clone()
        {
            return new LoaiTaiSanDto
            {
                Id = Id,
                Code = Code,
                Name = Name,
                Description = Description,
                IsActive = IsActive,
                MinValue=MinValue,
                MaxValue=MaxValue,
                DisplayOrder=DisplayOrder
            };
        }
    }
}
