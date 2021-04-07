using System.ComponentModel;

namespace WpfApp2.ViewModel
{
    public interface IViewModel : INotifyPropertyChanged
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
