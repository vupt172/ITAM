using ITAM.AppCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ITAM.AppCore.Interfaces
{
    public interface IToolbarAware
    {
        ToolbarContext ToolbarState { get; }
    }
}
