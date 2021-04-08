using DesktopFGApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdvancedCoding2
{
    public interface IClientModel : INotifyPropertyChanged
    {
        /***Properties***/
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
        int lineNumber
        {
            get;
            set;
        }
        string fpath
        {
            get;
            set;
        }
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
        float Rudder
        {
            get;
            set;
        }
        float Throttle
        {
            get;
            set;
        }
        float Airspeed
        {
            get;
            set;
        }
        float Altimeter
        {
            get;
            set;
        }
        float Pitch
        {
            get;
            set;
        }
        float Roll
        {
            get;
            set;
        }
        float Yaw
        {
            get;
            set;
        }
        float Heading
        {
            get;
            set;
        }
        List<string> HeaderNames
        {
            get;
            set;
        }
        string attributeChosen
        {
            get;
            set;
        }
        string Corralative
        {
            get;
            set;
        }
        List<List<string>> ListOfListOfAtt
        {
            get;
        }
        String[] CSVcopy
        {
            get;
            set;
        }
        /***Methods***/
        void connect();
        void xmlParser();
        double pearson(List<float> x, List<float> y, int size);
        void attSplit(string[] csvFile);
        Line linear_reg(List<DesktopFGApp.Model.Point> points, int size);
       
        }
}
