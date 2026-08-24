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
    public partial class NhaCungCapDto : ObservableValidator
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
        private string? phoneNumber;
        [ObservableProperty]
        private string? email;
        [ObservableProperty]
        private string? address;
        [ObservableProperty]
        private string? taxCode;

        public bool Validate()
        {
            ValidateAllProperties();
            return !HasErrors;
        }
        public NhaCungCapDto Clone()
        {
            return this.Adapt<NhaCungCapDto>();
        }
        //public NhaCungCapDto Clone()
        //{
        //    return new NhaCungCapDto
        //    {
        //        Id = Id,
        //        Code = Code,
        //        Name = Name,
        //        Description = Description,
        //        IsActive = IsActive,
        //        PhoneNumber = PhoneNumber,
        //        Email = Email,
        //        Address = Address,
        //        TaxCode= TaxCode
        //    };
        //}
    }
}
