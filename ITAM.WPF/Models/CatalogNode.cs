using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.WPF.Models
{
    public class CatalogNode
    {
        public string Title { get; set; } = string.Empty;

        public Type? ViewModelType { get; set; }

        public ObservableCollection<CatalogNode> Children { get; set; } = [];
        public bool IsLeaf => ViewModelType != null;
    }
}
