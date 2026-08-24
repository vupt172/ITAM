using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.WPF.Services.Interfaces
{
    public interface IErrorDialogService
    {
        void Show(Exception exception);
    }
}
