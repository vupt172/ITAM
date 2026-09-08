using CommunityToolkit.Mvvm.ComponentModel;

namespace ITAM.WPF.Models
{
    public partial class FeatureCheckItem : ObservableObject
    {
        public long FeatureId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        [ObservableProperty]
        private bool isSelected;
    }
}