using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ITAM.WPF.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        public virtual string Title => string.Empty;
    }
   
}
