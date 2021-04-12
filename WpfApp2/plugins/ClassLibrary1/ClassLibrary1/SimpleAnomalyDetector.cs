using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using DesktopFGApp;

namespace SimpleAnomalyDetector
{
    public class simpleAnomaly: IAnomalyDetector
    {
        public Dictionary<string, List<float>> AnomalyReport = new Dictionary<string, List<float>>();

        // need to insert user input for dll filr
        [DllImport("plugins\\timeseriesDLL\\Debug\\timeseriesDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Create();
        [DllImport("timeseriesDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void learnnig(IntPtr sad, string CSVfileName);
        [DllImport("timeseriesDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void detecting(IntPtr sad, IntPtr wrapAR, string detectfileName);
        [DllImport("timeseriesDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateVectorWrapper();
        [DllImport("timeseriesDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern long getTS(IntPtr vec, int x);
        [DllImport("timeseriesDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getDP(IntPtr vec, int x, StringBuilder attName);
        [DllImport("timeseriesDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getDPLen(IntPtr vec, int x);
        [DllImport("timeseriesDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int vectorSize(IntPtr vec);

       public void findAnomaly(string anomalyCSVPath)
        {
            IntPtr timeSeries = Create();
            learnnig(timeSeries, "reg_flight.csv");
            IntPtr anomalyVector = CreateVectorWrapper();
            detecting(timeSeries, anomalyVector, anomalyCSVPath);
            int vs = vectorSize(anomalyVector);
            for (int i = 0; i < vs; i++)
            {
                long val = getTS(anomalyVector, i);
                StringBuilder attName = new StringBuilder(getDPLen(anomalyVector, i));
                getDP(anomalyVector, i, attName);
                if (AnomalyReport.ContainsKey(attName.ToString()))
                {
                    AnomalyReport[attName.ToString()].Add(val);
                }
                else
                {
                    AnomalyReport.Add(attName.ToString(), new List<float>());
                    AnomalyReport[attName.ToString()].Add(val);
                }
            }
        }
        public Dictionary<string, List<float>> getAnomalyReport()
        {
            return AnomalyReport;
        }

    }
}
