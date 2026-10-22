using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.ViewModels.Reports
{
    public abstract partial class ReportViewModel : ObservableObject
    {
        public virtual async Task InitializeAsync() { }
    }
}
