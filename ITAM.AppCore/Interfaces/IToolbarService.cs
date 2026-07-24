using ITAM.AppCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.AppCore.Interfaces
{
    public interface IToolbarService
    {
        ToolbarContext CurrentState { get; set; }
        event Action<ToolbarContext>? StateChanged;
        void Apply(ToolbarContext toolbarState);
        void SetDefault();
    }
}
