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

        public float VM_rudder
        {
            get
            {
                return clientModel.Rudder;
            }
            set
            {
                if (VM_rudder != value)
                {
                    clientModel.Rudder = value;
                    onPropertyChanged("VM_rudder");
                }
            }
        }

        public float VM_throttle
        {
            get
            {
                return clientModel.Throttle;
            }
            set
            {
                if (VM_throttle != value)
                {
                    clientModel.Throttle = value;
                    onPropertyChanged("VM_throttle");
                }
            }
        }

        public float VM_airspeed
        {
            get
            {
                return clientModel.Airspeed;
            }
            set
            {
                if (VM_airspeed != value)
                {
                    clientModel.Airspeed = value;
                    onPropertyChanged("VM_airspeed");
                }
            }
        }

        public float VM_altimeter
        {
            get
            {
                return clientModel.Altimeter;
            }
            set
            {
                if (VM_altimeter != value)
                {
                    clientModel.Altimeter = value;
                    onPropertyChanged("VM_altimeter");
                }
            }
        }
        public float VM_roll
        {
            get
            {
                return clientModel.Roll;
            }
            set
            {
                if (VM_roll != value)
                {
                    clientModel.Roll = value;
                    onPropertyChanged("VM_roll");
                }
            }
        }
        public float VM_yaw
        {
            get
            {
                return clientModel.Yaw;
            }
            set
            {
                if (VM_yaw != value)
                {
                    clientModel.Yaw = value;
                    onPropertyChanged("VM_yaw");
                }
            }
        }

        public float VM_pitch
        {
            get
            {
                return clientModel.Pitch;
            }
            set
            {
                if (VM_pitch != value)
                {
                    clientModel.Pitch = value;
                    onPropertyChanged("VM_pitch");
                }
            }
        }
        public float VM_heading
        {
            get
            {
                return clientModel.Heading;
            }
            set
            {
                if (VM_heading != value)
                {
                    clientModel.Heading = value;
                    onPropertyChanged("VM_heading");
                }
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
