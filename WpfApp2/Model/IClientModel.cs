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
        //Transmission speed property
        int TransSpeed
        {
            get;
            set;
        }

        int simLen
        {
            get;
            set;
        }

        int currentTime
        {
            get;
            set;
        }

        int lineNumber
        {
            get;
            set;
        }

        //method to open socket with server
        void connect();
    }
}
