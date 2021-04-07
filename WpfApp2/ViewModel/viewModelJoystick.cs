using AdvancedCoding2;
using System.ComponentModel;

namespace WpfApp2.ViewModel
{
    public class viewModelJoystick : INotifyPropertyChanged
    {
        /***Data Members***/
        private IClientModel clientModel;
        public event PropertyChangedEventHandler PropertyChanged;
        /***Properties***/
        public float VM_aileron
        {
            get
            {
                return clientModel.Aileron; ;
            }
            set
            {
                if (VM_aileron != value)
                {
                    clientModel.Aileron = value;
                    onPropertyChanged("VM_aileron");
                }
            }
        }
        public float VM_elevator
        {
            get
            {
                return clientModel.Aileron;
            }
            set
            {
                if (VM_aileron != value)
                {
                    clientModel.Aileron = value;
                    onPropertyChanged("VM_elevator");
                }
            }
        }
        /***Methods***/
        public viewModelJoystick(IClientModel c)
        {
            this.clientModel = c;
            clientModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                onPropertyChanged("VM_" + e.PropertyName);
            };
        }
        public void onPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
