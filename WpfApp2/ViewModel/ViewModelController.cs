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
        private int currenTime;
        private int lineNum;
        private TimeSpan Time;
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

        public int VM_simLen
        {
            get
            {
                return clientModel.simLen;
            }
                       
        }

        public int VM_currentTime
        {
            get
            {
                 return clientModel.currentTime;
            }
            set
            {
                if(VM_currentTime != value)
                {
                    clientModel.currentTime = value;
                    onPropertyChanged("VM_playSpeed");
                }
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
                if(VM_Time != value)
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
                if(VM_lineNumber != value)
                {
                    clientModel.lineNumber = value;
                    onPropertyChanged("VM_lineNumber");
                }
            }
        }


        public ViewModelController(IClientModel m)
        {
            this.clientModel = m;
            playSpeed = clientModel.TransSpeed;
            Time = new TimeSpan(0, 0, 0);
            clientModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                onPropertyChanged("VM_" + e.PropertyName);
            };
            clientModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "currentTime" && clientModel.currentTime % 10 == 0)
                {
                    VM_Time.Add(new TimeSpan(0, 0, 1));
                }
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
           
            if ((connectThread.ThreadState & ThreadState.Suspended) == ThreadState.Suspended)
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
