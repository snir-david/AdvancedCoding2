using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedCoding2;

namespace WpfApp2.ViewModel
{
    public interface IViewModel: INotifyPropertyChanged
    {
        string VM_FGPath
        {
            get;
            set;
        }
        string VM_XMLPath
        {
            get;
            set;
        }
         
    }
}
