using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;

namespace AdvancedCoding2
{
    public class Client : IClientModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int port;
        private string server;
        private volatile int playSpeed;
        private int csvRowsNum;
        private volatile int lineNum;
        private volatile int currenTime;

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
                if(simLen != value)
                {
                    csvRowsNum = value;
                    NotifyPropertyChanged("simLen");
                }
            }
        }

        public int currentTime
        {
            get
            {
                return currenTime;
            }
            set
            {
                if (currentTime != value)
                {
                    currenTime = value;
                    NotifyPropertyChanged("currentTime");
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

                //getting number of rows
                String[] csvLine = File.ReadAllLines("C:\\Users\\snira\\RiderProjects" +
                                                        "\\Advanced Programming 2\\Advanced Programming 2\\reg_flight.csv");
                simLen = csvLine.Length;

                //read csv file 
                //StreamReader csvFile = new StreamReader("C:\\Users\\snira\\RiderProjects" +
                                                       // "\\Advanced Programming 2\\Advanced Programming 2\\reg_flight.csv");
                //csvFile.DiscardBufferedData();

                playSpeed = 100;
                currentTime = 0;
                //read first line
                //String line = csvFile.ReadLine();
                //keep reading until finish CSV file
                while (csvLine[lineNumber] != null)
                {
                    //read a line from CSV
                    csvLine[lineNumber] += "\n";
                    //Encode to bytes
                    Byte[] lineBytes = System.Text.Encoding.ASCII.GetBytes(csvLine[lineNumber]);
                    // Send the message to the connected TcpServer
                    stream.Write(lineBytes, 0, lineBytes.Length);
                    //read next line
                    lineNumber++;
                    //sleep for 100 mil-sec for sending ten times in a second
                    Thread.Sleep(playSpeed);
                    currentTime++;
                }

                // Close - file, stream and socket
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
            }
        }

    }
}

