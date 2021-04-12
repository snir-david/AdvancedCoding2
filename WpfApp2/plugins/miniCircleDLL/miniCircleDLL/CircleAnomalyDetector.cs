using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DesktopFGApp
{
    public class Point{
    	float x,y;
    	public Point(float x,float y) {
        this.x=x; 
         this.y= y;
        }
    }
    public class CircleAnomalyDetector : IAnomalyDetector
    {
        public Dictionary<string, List<int>> AnomalyReport = new Dictionary<string, List<int>>();
        public Dictionary<string, Tuple<Point,int>> Attfeatures = new Dictionary<string, Tuple<Point, int>>();

        // need to insert user input for dll filr
        [DllImport("plugins\\minCircleDll\\Debug\\minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Create();
        [DllImport("minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void learnnig(IntPtr sad, string CSVfileName);
        [DllImport("minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void detecting(IntPtr sad, IntPtr wrapAR, string detectfileName);
        [DllImport("minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateVectorWrapper();
        [DllImport("minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getTS(IntPtr vec, int x);
        [DllImport("minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getDP(IntPtr vec, int x, StringBuilder attName);
        [DllImport("minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getDPLen(IntPtr vec, int x);
        [DllImport("minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int vectorSize(IntPtr vec);
        [DllImport("minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getRadius(IntPtr cad);
        [DllImport("minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float getCenterX(IntPtr cad);
        [DllImport("minCircleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float getCenterY(IntPtr cad);
        

            public void findAnomaly(string anomalyCSVPath, List<string> headersList)
        {
            IntPtr CircleAnomalyDete = Create();
            learnnig(CircleAnomalyDete, "reg_flight.csv");
            var CSV = File.ReadAllLines(anomalyCSVPath);
            List<List<string>> csvWithHeaders = CSV.Select(x => x.Split(',').ToList()).ToList();
            csvWithHeaders.Insert(0, headersList);
            File.WriteAllLines("anomalyWithHeaders.csv", csvWithHeaders.Select(x => string.Join(",", x)));
            IntPtr anomalyVector = CreateVectorWrapper();
            detecting(CircleAnomalyDete, anomalyVector, "anomalyWithHeaders.csv");
            int vs = vectorSize(anomalyVector);
            for (int i = 0; i < vs; i++)
            {
                int val = getTS(anomalyVector, i);
                StringBuilder attName = new StringBuilder(getDPLen(anomalyVector, i));
                getDP(anomalyVector, i, attName);
                if (AnomalyReport.ContainsKey(attName.ToString()))
                {
                    AnomalyReport[attName.ToString()].Add(val);
                }
                else
                {
                    AnomalyReport.Add(attName.ToString(), new List<int>());
                    AnomalyReport[attName.ToString()].Add(val);
                    Attfeatures.Add(attName.ToString(), new Tuple<Point, int>(new Point(getCenterX(CircleAnomalyDete), getCenterY(CircleAnomalyDete)), getRadius(CircleAnomalyDete)));
                }
            }
        }
        public Dictionary<string, List<int>> getAnomalyReport()
        {
            return AnomalyReport;
        }
        public Dictionary<string, Tuple<Point, int>> getFeatures()
        {
            return Attfeatures;
        }
    }
}
