using DesktopFGApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using WpfApp2.ViewModel;
using System.Linq;


namespace AdvancedCoding2
{
    public class ViewModelController : IViewModel
    {
        /***Data Members***/
        private IClientModel clientModel;
        public bool isConnected, isRegLine, isCircel;
        private double playSpeed;
        private string FGPath, dllPath;
        private int dllChangeCounter;
        private Thread connectThread;
        private TimeSpan Time;
        public event PropertyChangedEventHandler PropertyChanged;
        public Dictionary<string, List<int>> VM_AnomalyReport;
        public Dictionary<string, Tuple<Point, int>> VM_Attfeatures;
        public dynamic dllAlgo;

        /***Properties***/
        public String[] VM_CSVcopy
        {
            get
            {
                return clientModel.CSVcopy;
            }
            set
            {
                if (VM_CSVcopy != value)
                    clientModel.CSVcopy = value;
            }
        }
        public List<List<string>> VM_currentAtt
        {
            get
            {
                return clientModel.ListOfListOfAtt;
            }

        }
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
        public string VM_DLLPath
        {
            get
            {
                return dllPath;
            }
            set
            {
                if (VM_DLLPath != value)
                    dllPath  = value;
            }
        }
        public List<string> VM_headerNames
        {
            get
            {
                return clientModel.HeaderNames;
            }
        }
        public int VM_dllCounter
        {
            get
            {
                return dllChangeCounter;
            }
            set
            {
                if(VM_dllCounter != value)
                {
                    dllChangeCounter = value;
                    onPropertyChanged("VM_dllCounter");
                }
            }
        }
        /***Methods***/
        public void onPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        public ViewModelController(IClientModel m)
        {
            this.clientModel = m;
            playSpeed = 0;
            VM_dllCounter = 0;
            Time = new TimeSpan(0, 0, 0);
            VM_AnomalyReport = new Dictionary<string, List<int>>();
            VM_Attfeatures = new Dictionary<string, Tuple<Point, int>>();
            isConnected = false;
            clientModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                onPropertyChanged("VM_" + e.PropertyName);
            };
        }
        public void connect()
        {
            //checking if there is path for FG
            if (VM_fpath != null)
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
            else
            {
                //if user didn't load any csv file
                MessageBox.Show("Please load a CSV and XML file before running the simulation", "File Missing", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /*setting up time - every second passed*/
        public void settingUpTime()
        {
            int min, sec, hours;
            //regular streaming is 10 line in second (sleep of 100 milsec) - calculating time accordingly
            sec = VM_lineNumber / 10;
            min = VM_lineNumber / 600;
            hours = VM_lineNumber / 6000;
            VM_Time = new TimeSpan(hours, min, sec);
        }
        public void resumeConnection()
        {
            if(connectThread != null)
                connectThread.Resume();
        }
        public void pauseConnection()
        {
            if (connectThread != null && connectThread.IsAlive)
                connectThread.Suspend();
        }
        /*copy XML if doesn't in protocol folder. User should select XML file*/
        public void copyXML()
        {
            //destfolder should be in protocol folder of FG folder
            string fileName = "\\data\\Protocol\\playback_small.xml";
            string destFile = VM_FGPath + fileName;
            //trying copy file
            try
            {
                File.Copy(VM_XMLPath, destFile, true);
            }
            catch (UnauthorizedAccessException)
            {
                //if user doesn't have any authorized to copy file
                MessageBox.Show("UnAuthorizedAccessException: Unable to access file.\nPleae Allow access and than try again.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void splitAtt()
        {
            clientModel.attSplit(this.VM_CSVcopy);
        }
        public void xmlPraser()
        {
            clientModel.xmlParser();
        }
        public void dllHandling()
        {
            var dllAlgorithm = Assembly.LoadFrom(VM_DLLPath);
            foreach(Type type in dllAlgorithm.GetExportedTypes())
            {
                if (type.Name.Contains("simple"))
                {
                    isRegLine = true;
                    isCircel = false;
                }  else if (type.Name.Contains("Circle"))
                {
                    isRegLine = false;
                    isCircel = true;
                }
                var interfaces = type.GetInterfaces();
                foreach (Type i in interfaces)
                {
                  if( i.Name == "IAnomalyDetector")
                  {
                        dynamic anomalyAlgo = Activator.CreateInstance(type);
                        dllAlgo = anomalyAlgo;
                        anomalyAlgo.findAnomaly(VM_fpath, VM_headerNames);     
                        VM_AnomalyReport = anomalyAlgo.getAnomalyReport();
                        VM_dllCounter += 1;
                  }
                }
            }
        }
    }
}
