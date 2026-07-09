using ITAM.WPF.Constants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.WPF.ViewModels.Catalogs
{
    public partial class HeThongDMViewModel : BaseViewModel
    {
        public override string Title => PageTitles.HeThongDM;
        public ObservableCollection<CatalogNode> Catalogs { get; } = [];

        public HeThongDMViewModel()
        {
            Catalogs = [
   new CatalogNode{Title = "Loại Tài Sản",ViewModelType = typeof(AssetTypeViewModel)},
    new CatalogNode
    {
        Title = "Danh Mục Tài Sản",
        ViewModelType = typeof(CategoryViewModel)
    },
    new CatalogNode
    {
        Title = "Phòng Ban",
        ViewModelType = typeof(DepartmentViewModel)
    },
    new CatalogNode
    {
        Title = "Vị Trí Phòng Ban",
        ViewModelType = typeof(RoomLocationViewModel)
    }
];
        }
    }
}
