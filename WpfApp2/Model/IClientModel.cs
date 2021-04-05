using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace AdvancedCoding2
{
    public interface IClientModel : INotifyPropertyChanged
    {
        //Transmission speed of data to server
        int TransSpeed
        {
            get;
            set;
        }

        //similutar lenght - giving CSV file checking number of rows
        int simLen
        {
            get;
            set;
        }

        
        //current "time" in stream file
        int lineNumber
        {
            get;
            set;
        }

        // CSV file path
        string fpath
        {
            get;
            set;
        }

        // XML file path
        string XMLpath
        {
            get;
            set;
        }

        float Aileron
        {
            get;
            set;
        }

        float Elevator
        {
            get;
            set;
        }

        List<string> HeaderNames
        {
            get;
            set;     
        }

        //method to open socket with server
        void connect();
    }
}
