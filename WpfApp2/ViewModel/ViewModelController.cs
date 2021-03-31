using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;


namespace AdvancedCoding2
{
    public class ViewModelController : INotifyPropertyChanged
    {
        private IClientModel clientModel;
        public bool isConnected = false;
        private double playSpeed = 0;
        private Thread connectThread;
        private TimeSpan Time;
        public event PropertyChangedEventHandler PropertyChanged;


        public double VM_playSpeed
        {
            get
            {
                return playSpeed;
            }
            set
            {
                if (playSpeed != value)
                {
                    playSpeed = value;
                    onPropertyChanged("VM_playSpeed");
                }
            }
        }

        public int VM_TransSpeed
        {
            get
            {
                return clientModel.TransSpeed;
            }
            set
            {
                if (VM_playSpeed != value)
                    clientModel.TransSpeed = value;
            }
        }

        public int VM_simLen
        {
            get
            {
                return clientModel.simLen;
            }

        }


        public TimeSpan VM_Time
        {
            get
            {
                return Time;
            }
            set
            {
                if (VM_Time != value)
                {
                    Time = value;
                    onPropertyChanged("VM_Time");
                }
            }
        }

        public int VM_lineNumber
        {
            get
            {
                return clientModel.lineNumber;
            }
            set
            {
                if (VM_lineNumber != value)
                {
                    clientModel.lineNumber = value;
                    onPropertyChanged("VM_lineNumber");
                }
            }
        }

      

        public ViewModelController(IClientModel m)
        {
            this.clientModel = m;
           Time = new TimeSpan(0, 0, 0);
            clientModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                onPropertyChanged("VM_" + e.PropertyName);
            };

        }

        public void connect()
        {

            //checking if thread is already exist and alive - if not creating new thread for connection
            if (connectThread == null || !connectThread.IsAlive)
            {
                connectThread = new Thread(delegate ()
                {
                    clientModel.connect();
                    isConnected = false;
                });
            }

            //if thread is suspend - resume thread
            if ((connectThread.ThreadState & ThreadState.Suspended) == ThreadState.Suspended)
            {
                resumeConnection();
            }
            else //start connection
            {
                isConnected = true;
                connectThread.Start();
                
            }
        }

        public void settingUpTime()
        {
            int min, sec, hours;
            sec = VM_lineNumber / 10;
            min = VM_lineNumber / 600;
            hours = VM_lineNumber / 6000;
            VM_Time = new TimeSpan(hours, min, sec);
        }

        public void resumeConnection()
        {
            connectThread.Resume();
        }
        public void pauseConnection()
        {
            connectThread.Suspend();
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
