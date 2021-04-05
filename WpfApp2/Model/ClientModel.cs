using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Xml;
using System.Collections.Generic;

namespace AdvancedCoding2
{
    public class Client : IClientModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int port, csvRowsNum;
        private volatile int playSpeed, lineNum;
        private string server, copyLine;
        private volatile string filePath, xmlPath;
        private List<string> chunksName, ailList= new List<string>(), elvList = new List<string>(), rudList = new List<string>(), thrList = new List<string>(),
            airList = new List<string>(), altList = new List<string>(), rolList = new List<string>(), pitList = new List<string>(), yawList = new List<string>(), headingList = new List<string>();
        private List<List<string>> currAtt = new List<List<string>>();

        private int aileronInx, elevatorInx, rudderInx, throttleInx, airspeedInx, altimeterInx, rollInx, pitchInx, yawInx, headingInx;
        private volatile float aileron, elevator, rudder, throttle, airspeed, altimeter, roll, pitch, yaw, heading;


        public int TransSpeed { 
            get
            {
                return playSpeed;
            }
            set
            {
                if(TransSpeed != value)
                {
                    playSpeed = value;
                    NotifyPropertyChanged("playSpeed");
                }
            }
        }

        public int simLen
        {
            get
            {
                return csvRowsNum;
            }
            set
            {
                if (simLen != value)
                {
                    csvRowsNum = value;
                    NotifyPropertyChanged("simLen");
                }
            }
        }
      
        public int lineNumber
        {
            get
            {
                return lineNum;
            }
            set
            {
                if (lineNumber != value)
                {
                    lineNum = value;
                    NotifyPropertyChanged("lineNumber");
                }
            }
        }

        public string fpath
        {
            get
            {
                return filePath;
            }
            set
            {
                if (fpath != value)
                    filePath = value;
            }
        }

        public string XMLpath
        {
            get
            {
                return xmlPath;
            }
            set
            {
                if (XMLpath != value)
                    xmlPath = value;
            }
        }
        public float Aileron
        {
            get
            {
                return aileron;
            }
            set
            {
                if (Aileron != value)
                {
                    aileron = value;
                    NotifyPropertyChanged("aileron");
                }
            }
        }

        public float Elevator
        {
            get
            {
                return elevator;
            }
            set
            {
                if (Elevator != value)
                {
                    elevator = value;
                    NotifyPropertyChanged("elevator");
                }
            }
        }

        public float Rudder
        {
            get
            {
                return rudder;
            }
            set
            {
                if (Rudder != value)
                {
                    rudder = value;
                    NotifyPropertyChanged("rudder");
                }
            }
        }

        public float Throttle
        {
            get
            {
                return throttle;
            }
            set
            {
                if (Throttle != value)
                {
                    throttle = value;
                    NotifyPropertyChanged("throttle");
                }
            }
        }

        public float Airspeed
        {
            get
            {
                return airspeed;
            }
            set
            {
                if (Airspeed != value)
                {
                    airspeed = value;
                    NotifyPropertyChanged("airspeed");
                }
            }
        }
        public float Altimeter
        {
            get
            {
                return altimeter;
            }
            set
            {
                if (Altimeter != value)
                {
                    altimeter = value;
                    NotifyPropertyChanged("altimeter");
                }
            }
        }

        public float Roll
        {
            get
            {
                return roll;
            }
            set
            {
                if (Roll != value)
                {
                    roll = value;
                    NotifyPropertyChanged("roll");
                }
            }
        }

        public float Pitch
        {
            get
            {
                return pitch;
            }
            set
            {
                if (Pitch != value)
                {
                    pitch = value;
                    NotifyPropertyChanged("pitch");
                }
            }
        }

        public float Yaw
        {
            get
            {
                return yaw;
            }
            set
            {
                if (Yaw != value)
                {
                    yaw = value;
                    NotifyPropertyChanged("yaw");
                }
            }
        }

        public float Heading
        {
            get
            {
                return heading;
            }
            set
            {
                if (Heading != value)
                {
                    heading = value;
                    NotifyPropertyChanged("heading");
                }
            }
        }
        public List<string> HeaderNames
        {
            get
            {
                return chunksName;
            }
        }
        public List<List<string>> CurrentAtt
        {
            get
            {
                return currAtt;
            }
        }

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public Client(String server, int port)
        {
            this.port = port;
            this.server = server;
        }

        public void xmlParser()
        {
            chunksName = new List<string>();
            XmlDocument doc = new XmlDocument();
            doc.Load(XMLpath);
            XmlNode node = doc.DocumentElement.SelectSingleNode("/PropertyList/generic/output");
            foreach (XmlNode n in node)
            {
                if (n.LocalName.Equals("chunk"))
                {
                    string name = n.SelectSingleNode("name").InnerText;
                    chunksName.Add(name);
                    currAtt.Add(new List<string>());

                }
            }
            
        }

        public void attSplit(string[] csvFile)
        {
            foreach(string line in csvFile)
            {
                string[] curr = line.Split(',');
                for (int i = 0; i < curr.Length; i++)
                {
                    currAtt[i].Add(curr[i]);
                }
            }
            
        }

        public void joyStickPos()
        {
            ailList = CurrentAtt[aileronInx];
            elvList = CurrentAtt[elevatorInx];
            float ail = float.Parse(ailList[lineNumber]);
            float elev = float.Parse(elvList[lineNumber]);
            Aileron = ail * 50 + 78;
            Elevator = elev * 50 + 78;
        }
        public void initJoystick()
        {
            aileronInx = HeaderNames.FindIndex(a => a.Contains("aileron"));
            elevatorInx = HeaderNames.FindIndex(a => a.Contains("elevator"));
            Aileron = 78;
            Elevator = 78;
        }

        public void ruddersPos()
        {
            rudList = CurrentAtt[rudderInx];
            thrList = CurrentAtt[throttleInx];
            airList = CurrentAtt[airspeedInx];
            altList = CurrentAtt[altimeterInx];
            rolList = CurrentAtt[rollInx];
            pitList = CurrentAtt[pitchInx];
            yawList = CurrentAtt[yawInx];
            headingList = CurrentAtt[headingInx];

            Airspeed = float.Parse(airList[lineNumber]);
            Altimeter = float.Parse(altList[lineNumber]);
            Roll = float.Parse(rolList[lineNumber]);
            Pitch = float.Parse(pitList[lineNumber]);
            Yaw = float.Parse(yawList[lineNumber]);
            Heading = float.Parse(headingList[lineNumber]);
            float rudd = float.Parse(rudList[lineNumber]);
            float throttle = float.Parse(thrList[lineNumber]);
            Rudder = rudd * 108 + 108;
            Throttle = throttle * -226 + 226;
            
        }

        public void initRudders()
        {
            rudderInx = HeaderNames.FindIndex(a => a.Contains("rudder"));
            throttleInx = HeaderNames.FindIndex(a => a.Contains("throttle"));
            airspeedInx = HeaderNames.FindIndex(a => a.Contains("airspeed-kt"));
            altimeterInx = HeaderNames.FindIndex(a => a.Contains("altimeter_indicated-altitude-ft"));
            rollInx = HeaderNames.FindIndex(a => a.Contains("roll-deg"));
            pitchInx = HeaderNames.FindIndex(a => a.Contains("pitch-deg"));
            yawInx = HeaderNames.FindIndex(a => a.Contains("side-slip-deg"));
            headingInx = HeaderNames.FindIndex(a => a.Contains("heading-deg"));


            Rudder = 108;
            Throttle = 108;
            Airspeed = 0;
            Altimeter = 0;
            Roll = 0;
            Pitch = 0;
            Yaw = 0;
            Heading = 0;

        }

        public void connect()
        {
            try
            {
                
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer
                // connected to the same address as specified by the server, port
                // combination.
                TcpClient client = new TcpClient(server, port);
                // Get a client stream for reading and writing.
                var stream = client.GetStream();

                // reading csv file into string array of lines
                String[] csvLine = File.ReadAllLines(fpath);
                String[] csvCopy = csvLine;
                // getting number of rows
                simLen = csvLine.Length;
                this.xmlParser();
                //setting up playing speed to 100 mill-sec
                playSpeed = 100;
                initJoystick();
                initRudders();
                attSplit(csvCopy);

                // sending one line at a time to server
                while (simLen > lineNumber)
                {
                    //get a line from array
                    csvLine[lineNumber] += "\n";
                    //Encode to bytes
                    Byte[] lineBytes = System.Text.Encoding.ASCII.GetBytes(csvLine[lineNumber]);
                    // Send the message to the connected TcpServer
                    stream.Write(lineBytes, 0, lineBytes.Length);
                    joyStickPos();
                    ruddersPos();
                    //inc index to next line
                    lineNumber++;
                    //sleep for playspeed mil-sec for sending ten times in a second
                    Thread.Sleep(playSpeed);
                }

                // Close - stream and socket
                stream.Close();
                client.Close();
            }
            //catch problem with port or server ip
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            //catch problem with socket
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                MessageBox.Show("Please make sure to open FlightGear simulator with following additional setting and than try again:\n" +
                    "--generic=socket,in,10,127.0.0.1,5400,tcp,playback_small\n--fdm = null" , "Couldn't Reach Server", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

    }
}

