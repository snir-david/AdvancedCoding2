using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Xml;
using System.Collections.Generic;
using DesktopFGApp.Model;

namespace AdvancedCoding2
{
    public class Client : IClientModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int port, csvRowsNum;
        private volatile int playSpeed, lineNum;
        private string server, copyLine , chosen , corralative;
        private volatile string filePath, xmlPath;
        private List<string> chunksName, ailList= new List<string>(), elvList = new List<string>();
        private List<List<string>> currAtt = new List<List<string>>();

        private String[] csvCopy;

        private int aileronInx, elevatorInx;
        private volatile float aileron, elevator;


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

        public List<string> HeaderNames
        {
            get
            {
                return chunksName;
            }
            set
            {
                if (HeaderNames != value)
                {
                    chunksName = value;
                    NotifyPropertyChanged("HeaderNames");
                }

            }
        }
        public List<List<string>> CurrentAtt
        {
            get
            {
                return currAtt;
            }
        }

        // i added it in order to call splitAtt from view model controller.
        public String[] CSVcopy
        {
            get
            {
                return csvCopy;
            }
            set
            {
                if (CSVcopy != value)
                {
                    csvCopy = value;
                    NotifyPropertyChanged("CSVcopy");
                }
            }
        }

        // this property is the chosen data (the button that had been pressed).
        public string Chosen
        {
            get
            {
                return chosen; ;
            }
            set
            {
                if (Chosen != value)
                    chosen = value;
            }
        }

        // this property is the corralative data.
        public string Corralative
        {
            get
            {
                return corralative; ;
            }
            set
            {
                if (Corralative != value)
                    corralative = value;
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
                    //currAtt.Add(new List<string>());

                }
            }
            
        }

        public void attSplit(string[] csvFile)
        {
            int counter = 0;

            foreach (string line in csvFile)
            {
                string[] curr = line.Split(',');    
                for (int i = 0; i < curr.Length; i++)
                {
                    if(counter <= curr.Length - 1)
                    {
                        currAtt.Add(new List<string>());
                        counter++;
                    }           
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
            Aileron = ail * 30 + 48;
            Elevator = elev * 30 + 48;
        }
        public void initJoystick()
        {
            aileronInx = HeaderNames.FindIndex(a => a.Contains("aileron"));
            elevatorInx = HeaderNames.FindIndex(a => a.Contains("elevator"));
            Aileron = 78;
            Elevator = 78;
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
                //String[] csvCopy = csvLine;
                this.csvCopy = csvLine;

                // getting number of rows
                simLen = csvLine.Length;
                //this.xmlParser();
                //setting up playing speed to 100 mill-sec
                playSpeed = 100;
                initJoystick();
                //attSplit(csvCopy);

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

        // ****************** shani's functions *******************//

        private float avg(List<float> x, int size)
        {
            float sum = 0;
            for (int i = 0; i < size; sum += x[i], i++) ;
            return sum / size;
        }

        // returns the variance of X and Y
        private float var(List<float> x, int size)
        {
            float av = avg(x, size);
            float sum = 0;
            for (int i = 0; i < size; i++)
            {
                sum += x[i] * x[i];
            }
            return sum / size - av * av;
        }

        // returns the Covariance of X and Y
        private float cov(List<float> x, List<float> y, int size)
        {
            float sum = 0;
            for (int i = 0; i < size; i++)
            {
                sum += x[i] * y[i];
            }
            sum /= size;

            return sum - avg(x, size) * avg(y, size);
        }


        // returns the Pearson correlation coefficient of X and Y
        public double pearson(List<float> x, List<float> y, int size)
        {
            /*if (Math.Sqrt(var(x, size)) * Math.Sqrt(var(y, size)) == 0)
            {
                return 0;
            }*/
            
            return cov(x, y, size) / (Math.Sqrt(var(x, size)) * Math.Sqrt(var(y, size)));
        }

        // performs a linear regression and returns the line equation
        Line linear_reg(List<DesktopFGApp.Model.Point> points, int size)
        {
            List<float> x = new List<float>();
            List<float> y = new List<float>();
            for (int i = 0; i < size; i++)
            {
                x[i] = points[i].x;
                y[i] = points[i].y;
            }
            float a = cov(x, y, size) / var(x, size);
            float b = avg(y, size) - a * (avg(x, size));

            Line l = new Line(a , b);
            
            return l;
        }

    }

}

