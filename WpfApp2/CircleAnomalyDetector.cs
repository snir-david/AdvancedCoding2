using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DesktopFGApp
{
    class CircleAnomalyDetector: IAnomalyDetector
    {
        public Dictionary<string, List<float>> AnomalyReport = new Dictionary<string, List<float>>();

        // need to insert user input for dll filr
        [DllImport("WpfApp2\\plugins\\minCircleDll\\Debug\\minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Create();
        [DllImport("WpfApp2\\plugins\\minCircleDll\\Debug\\minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void learnnig(IntPtr sad, string CSVfileName);
        [DllImport("WpfApp2\\plugins\\minCircleDll\\Debug\\minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void detecting(IntPtr sad, IntPtr wrapAR, string detectfileName);
        [DllImport("WpfApp2\\plugins\\minCircleDll\\Debug\\minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateVectorWrapper();
        [DllImport("WpfApp2\\plugins\\minCircleDll\\Debug\\minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern long getTS(IntPtr vec, int x);
        [DllImport("WpfApp2\\plugins\\minCircleDll\\Debug\\minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getDP(IntPtr vec, int x, StringBuilder attName);
        [DllImport("WpfApp2\\plugins\\minCircleDll\\Debug\\minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getDPLen(IntPtr vec, int x);
        [DllImport("WpfApp2\\plugins\\minCircleDll\\Debug\\minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int vectorSize(IntPtr vec);


        public void findAnomaly(string flightCSVPath)
        {
            IntPtr timeSeries = Create();
            learnnig(timeSeries, "WpfApp2\\reg_flight.csv");
            IntPtr anomalyVector = CreateVectorWrapper();
            detecting(timeSeries, anomalyVector, flightCSVPath);
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
