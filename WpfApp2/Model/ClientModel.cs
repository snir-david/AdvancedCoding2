using DesktopFGApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Xml;

namespace AdvancedCoding2
{
    public class Client : IClientModel
    {
        /***Data Members***/
        public event PropertyChangedEventHandler PropertyChanged;
        private int port, csvRowsNum, aileronInx, elevatorInx;
        private string server, attChosen, corrToChose;
        private List<string> chunksName, ailList, elvList;
        private List<List<string>> currAtt;
        private String[] csvCopy;
        //variables that need to be accessable outside of thread
        private volatile string filePath, xmlPath;
        private volatile float aileron, elevator;
        private volatile int playSpeed, lineNum;
        /***Properties***/
        //properties that we want to notify changed
        public int TransSpeed
        {
            get
            {
                return playSpeed;
            }
            set
            {
                if (TransSpeed != value)
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
        //properties not suposed to changed during running
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
        public List<List<string>> ListOfListOfAtt
        {
            get
            {
                return currAtt;
            }
        }
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
                }
            }
        }
        public string attributeChosen
        {
            get
            {
                return attChosen; ;
            }
            set
            {
                if (attributeChosen != value)
                    attChosen = value;
            }
        }
        public string Corralative
        {
            get
            {
                return corrToChose; ;
            }
            set
            {
                if (Corralative != value)
                    corrToChose = value;
            }
        }
        /***Methods***/
        //notify on change function
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        //constructor - getting server and port number
        public Client(String server, int port)
        {
            this.port = port;
            this.server = server;
            //intiliaze lists
            ailList = new List<string>();
            elvList = new List<string>();
            currAtt = new List<List<string>>();
        }
        /*taking xml file and exctracting headers name (only output chucnks) */
        public void xmlParser()
        {
            chunksName = new List<string>();
            XmlDocument doc = new XmlDocument();
            doc.Load(XMLpath);
            //finding the output node 
            XmlNode node = doc.DocumentElement.SelectSingleNode("/PropertyList/generic/output");
            //iterating all childer nodes in output node
            foreach (XmlNode n in node)
            {
                //checking if node names equals to chunck - if it is - taking name attribute from chunck
                if (n.LocalName.Equals("chunk"))
                {
                    string name = n.SelectSingleNode("name").InnerText;
                    chunksName.Add(name);
                }
            }

        }
        /*getting csv file as string array and splitting it by the "," */
        public void attSplit(string[] csvFile)
        {
            int counter = 0;
            //iterating array - splitting every cell (line) in array
            foreach (string line in csvFile)
            {
                string[] curr = line.Split(',');
                //adding attributes to the right header list
                for (int i = 0; i < curr.Length; i++)
                {
                    if (counter <= curr.Length - 1)
                    {
                        currAtt.Add(new List<string>());
                        counter++;
                    }
                    currAtt[i].Add(curr[i]);
                }
            }
        }
        /*sending values of x and y for jostick movement*/
        public void joyStickPos()
        {
            ailList = ListOfListOfAtt[aileronInx];
            elvList = ListOfListOfAtt[elevatorInx];
            //convert string to float
            float ail = float.Parse(ailList[lineNumber]);
            float elev = float.Parse(elvList[lineNumber]);
            //"normalized" value for joystick postion
            Aileron = ail * 30 + 48;
            Elevator = elev * 30 + 48;
        }
        /*inti joystick postion - finding aileron and elevator index from list of list */
        public void initJoystick()
        {
            aileronInx = HeaderNames.FindIndex(a => a.Contains("aileron"));
            elevatorInx = HeaderNames.FindIndex(a => a.Contains("elevator"));
            Aileron = 78;
            Elevator = 78;
        }
        /* connect to server and start stream data*/
        public void connect()
        {
            try
            {
                // Create a TcpClient. Note, for this client to work you need to have a TcpServer
                // connected to the same address as specified by the server, port combination.
                TcpClient client = new TcpClient(server, port);
                //Get a client stream for reading and writing.
                var stream = client.GetStream();
                //reading csv file into string array of lines - open and close the file
                String[] csvLine = File.ReadAllLines(fpath);
                //getting number of rows
                simLen = csvLine.Length;
                //setting up playing speed to 100 mill-sec - 10 lines in second
                TransSpeed = 100;
                initJoystick();
                // while loope - as long there is data to send - send one line at a time to server
                while (simLen > lineNumber)
                {
                    //adding end of line to the current line
                    csvLine[lineNumber] += "\n";
                    //Encode to bytes
                    Byte[] lineBytes = System.Text.Encoding.ASCII.GetBytes(csvLine[lineNumber]);
                    // Send the message to the connected TcpServer
                    stream.Write(lineBytes, 0, lineBytes.Length);
                    //calculating joystick position
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
                    "--generic=socket,in,10,127.0.0.1,5400,tcp,playback_small\n--fdm = null", "Couldn't Reach Server", MessageBoxButton.OK, MessageBoxImage.Error);
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
            return cov(x, y, size) / (Math.Sqrt(var(x, size)) * Math.Sqrt(var(y, size)));
        }
        // performs a linear regression and returns the line equation
        public Line linear_reg(List<DesktopFGApp.Model.Point> points, int size)
        {
            List<float> x = new List<float>();
            List<float> y = new List<float>();
            for (int i = 0; i < size; i++)
            {
                x.Add(points[i].x);
                y.Add(points[i].y);
            }
            float a = cov(x, y, size) / var(x, size);
            float b = avg(y, size) - a * (avg(x, size));
            Line l = new Line(a, b);
            return l;
        }
    }
}

