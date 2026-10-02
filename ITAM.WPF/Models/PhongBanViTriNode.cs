using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Models
{
    public partial class PhongBanViTriNode : ObservableObject
    {
        public string Code { get; init; } = "";
        public string Name { get; init; } = "";
        public bool IsPhongBan { get; init; }
        public bool IsSystem { get; init; }
        public ObservableCollection<PhongBanViTriNode> Children { get; } = new();

        [ObservableProperty] private bool _isExpanded;
    }
}
