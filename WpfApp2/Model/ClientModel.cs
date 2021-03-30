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
        private int playSpeed;
        public int TransSpeed
        {
            get
            {
                return this.playSpeed;
            }
            set
            {
                if (value != this.playSpeed)
                {
                    this.playSpeed = value;
                    NotifyPropertyChanged("playSpeed");
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
            this.playSpeed = 100;
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

                //read csv file 
                StreamReader csvFile = new StreamReader("C:\\Users\\snira\\RiderProjects" +
                                                        "\\Advanced Programming 2\\Advanced Programming 2\\reg_flight.csv");
                //read first line
                String line = csvFile.ReadLine();
                //keep reading until finish CSV file
                while (line != null)
                {
                    //read a line from CSV
                    line += "\n";
                    //Encode to bytes
                    Byte[] lineBytes = System.Text.Encoding.ASCII.GetBytes(line);
                    // Send the message to the connected TcpServer
                    stream.Write(lineBytes, 0, lineBytes.Length);
                    //read next line
                    line = csvFile.ReadLine();
                    //sleep for 100 mil-sec for sending ten times in a second
                    Thread.Sleep(playSpeed);
                }

                // Close - file, stream and socket
                csvFile.Close();
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

        // public static void Main()
        // {
        //     Client c = new Client();
        //     c.connect("127.0.0.1", 5400);
        //  }
    }
}

