using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedCoding2;

namespace WpfApp2.ViewModel
{
    public class viewModelJoystick: INotifyPropertyChanged
    {
        private IClientModel clientModel;
        public event PropertyChangedEventHandler PropertyChanged;

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

        public string VM_headerNames
        {
            get
            {
                Console.WriteLine("im here");
                return clientModel.HeaderNames[0];
            }
        }
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
