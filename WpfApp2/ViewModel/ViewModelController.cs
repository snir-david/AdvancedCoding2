using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Windows;
using System.IO;
using WpfApp2.ViewModel;

namespace AdvancedCoding2
{
    public class ViewModelController : IViewModel
    {
        private IClientModel clientModel;
        public viewModelJoystick joy;
        public bool isConnected = false;
        private double playSpeed = 0;
        private string FGPath;
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

        public string VM_fpath
        {
            get
            {
                return clientModel.fpath;
            }
            set
            {
                if (VM_fpath != value)
                {
                    clientModel.fpath = value;
                    onPropertyChanged("VM_path");
                }
                    

            }
        }

        public string VM_XMLPath
        {
            get
            {
                return clientModel.XMLpath;
            }
            set
            {
                if (VM_XMLPath != value)
                    clientModel.XMLpath = value;
            }
        }

        public string VM_FGPath
        {
            get
            {
                return FGPath;
            }
            set
            {
                if (VM_FGPath != value)
                    FGPath = value;
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
            if(VM_fpath != null)
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
            } else
            {
                MessageBox.Show("Please load a CSV and XML file before running the simulation","File Missing", MessageBoxButton.OK, MessageBoxImage.Error);

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
            if(connectThread != null)
                connectThread.Suspend();
        }

        public void copyXML()
        {
            string fileName = "\\data\\Protocol\\playback_small.xml";
            string destFile = VM_FGPath+ fileName;
            try
            {
                File.Copy(VM_XMLPath, destFile, true);
            } catch (UnauthorizedAccessException e)
            {
                MessageBox.Show("UnAuthorizedAccessException: Unable to access file.\nPleae Allow access and than try again.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
