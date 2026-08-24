using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ITAM.AppCore.DTOs
{
    public partial class DMTaiSanDto : ObservableValidator
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
        private bool isTrackedById;
        [ObservableProperty]
        private bool requireSerial;
        [ObservableProperty]
        private int displayOrder = 0;
        public bool Validate()
        {
            ValidateAllProperties();
            return !HasErrors;
        }
        public DMTaiSanDto Clone()
        {
            return new DMTaiSanDto
            {
                Id = Id,
                Code = Code,
                Name = Name,
                Description = Description,
                IsActive = IsActive,
                IsTrackedById = IsTrackedById,
                RequireSerial = RequireSerial,
                DisplayOrder = DisplayOrder
            };
        }

    }
}
