using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Interfaces;
using ITAM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.ViewModels
{
    // ITAM.WPF/ViewModels/PhongBanViTriTreeViewModel.cs
    public partial class PhongBanViTriTreeViewModel : ObservableObject
    {
        private readonly ICatalogService<PhongBan> _phongBanService;
        private readonly ICatalogService<ViTriTaiSan> _viTriService;

        public ObservableCollection<PhongBanViTriNode> Roots { get; } = new();

        public PhongBanViTriTreeViewModel(
            ICatalogService<PhongBan> phongBanService,
            ICatalogService<ViTriTaiSan> viTriService)
        {
            _phongBanService = phongBanService;
            _viTriService = viTriService;
        }

        public async Task InitializeAsync()
        {
            var phongBans = await _phongBanService.GetAllAsync();   // TODO xác nhận tên method
            var viTris = await _viTriService.GetAllAsync();         // TODO xác nhận tên method

            var viTriTheoPhongBan = viTris.ToLookup(v => v.PhongBanId);

            Roots.Clear();
            foreach (var pb in phongBans)
            {
                var children = viTriTheoPhongBan[pb.Id]
                    .OrderBy(v => v.Name)
                    .Select(v => new PhongBanViTriNode
                    {
                        Code = v.Code,
                        Name = v.Name,
                        IsSystem = v.IsSystem
                    })
                    .ToList();

                var node = new PhongBanViTriNode
                {
                    Code = pb.Code,
                    Name = pb.Name,
                    IsPhongBan = true,
                    IsSystem = children.Any(c => c.IsSystem)
                };
                foreach (var c in children) node.Children.Add(c);
                Roots.Add(node);
            }

            // Phòng ban thường lên trước, 4 kho hệ thống xuống cuối
            var sorted = Roots.OrderBy(r => r.IsSystem).ThenBy(r => r.Name).ToList();
            Roots.Clear();
            foreach (var r in sorted) Roots.Add(r);
        }

        [RelayCommand]
        private void ExpandAll() { foreach (var r in Roots) r.IsExpanded = true; }

        [RelayCommand]
        private void CollapseAll() { foreach (var r in Roots) r.IsExpanded = false; }
    }
}
