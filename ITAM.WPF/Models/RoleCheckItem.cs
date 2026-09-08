using CommunityToolkit.Mvvm.ComponentModel;

namespace ITAM.WPF.Models
{
    public partial class RoleCheckItem : ObservableObject
    {
        public long RoleId { get; set; }
        public string RoleName { get; set; } = null!;

        [ObservableProperty]
        private bool isSelected;
    }
}