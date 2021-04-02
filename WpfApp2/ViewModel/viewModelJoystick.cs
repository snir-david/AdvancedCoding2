using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedCoding2;

namespace WpfApp2.ViewModel
{
    class viewModelJoystick
    {
        private Client clientModel;
        public event PropertyChangedEventHandler PropertyChanged;
        private float aileron, elevator;
        private int aileronInx, elevatorInx;
        public float VM_aileron
        {
            get
            {
                return aileron;
            }
            set
            {
                if (aileron != value)
                {
                    aileron = value;
                    onPropertyChanged("VM_aileron");
                }
            }
        }

        public float VM_elevator
        {
            get
            {
                return elevator;
            }
            set
            {
                if (elevator != value)
                {
                    elevator = value;
                    onPropertyChanged("VM_elevator");
                }
            }
        }


        public void joyStickPos()
        {
            float ail = float.Parse(clientModel.CurrentAtt[aileronInx][clientModel.lineNumber]);
            float elev = float.Parse(clientModel.CurrentAtt[elevatorInx][clientModel.lineNumber]);
            VM_aileron = ail * 75 + 125;
            VM_elevator = elev * 75 + 125;
        }
        public void initJoystick()
        {
            aileronInx = clientModel.HeaderNames.FindIndex(a => a.Contains("aileron"));
            elevatorInx = clientModel.HeaderNames.FindIndex(a => a.Contains("elevator"));
            VM_aileron = 125;
            VM_elevator = 125;
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
