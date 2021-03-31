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
        private Thread connectThread;
        public event PropertyChangedEventHandler PropertyChanged;


        private double playSpeed = 1;
        public double VM_playSpeed
        {
            get
            {
                return playSpeed;
            }
            set
            {
                if(playSpeed != value)
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
                if(VM_playSpeed != value)
                    clientModel.TransSpeed = value;
            }
        }

        public ViewModelController(IClientModel m)
        {
            this.clientModel = m;
            playSpeed = clientModel.TransSpeed;
            clientModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                onPropertyChanged("VM_" + e.PropertyName);
            };
        }
        
        public void connect()
        {
            if(connectThread == null || !connectThread.IsAlive)
            {
                connectThread = new Thread(delegate ()
                {
                    clientModel.connect();
                    isConnected = false;
                });
            }
           
            else if ((connectThread.ThreadState & ThreadState.Suspended) == ThreadState.Suspended)
            {
                resumeConnection();
            } else
            {
                isConnected = true;
                connectThread.Start();
            }
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
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }

}
