using CommunityToolkit.Mvvm.ComponentModel;

namespace ITAM.WPF.Models
{
    public partial class PhongBanCheckItem : ObservableObject
    {
        public long PhongBanId { get; set; }
        public string Code { get; set; } = null!;
        public string PhongBanName { get; set; } = null!;

        [ObservableProperty]
        private bool isSelected;
    }
}