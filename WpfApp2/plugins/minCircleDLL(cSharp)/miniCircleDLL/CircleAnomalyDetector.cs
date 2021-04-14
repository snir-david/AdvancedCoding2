using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DesktopFGApp
{
    class Point{
    	float x,y;
    	public Point(float x,float y) {
        this.x=x; 
         this.y= y;
        }
    }
    public class CircleAnomalyDetector : IAnomalyDetector
    {
        /***Data Members***/
        public Dictionary<string, List<int>> AnomalyReport = new Dictionary<string, List<int>>();
        public int radius;
        public double centerX, centerY;
        public IntPtr CircleAnomalyDete;
        /*** DLL imports from C++ **/
        [DllImport("plugins\\minCircleDll(c++)\\Debug\\minCircleDll(c++).dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Create();
        [DllImport("minCircleDll(c++).dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void learnnig(IntPtr sad, string CSVfileName);
        [DllImport("minCircleDll(c++).dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void detecting(IntPtr sad, IntPtr wrapAR, string detectfileName);
        [DllImport("minCircleDll(c++).dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateVectorWrapper();
        [DllImport("minCircleDll(c++).dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getTS(IntPtr vec, int x);
        [DllImport("minCircleDll(c++).dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getDP(IntPtr vec, int x, StringBuilder attName);
        [DllImport("minCircleDll(c++).dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getDPLen(IntPtr vec, int x);
        [DllImport("minCircleDll(c++).dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int vectorSize(IntPtr vec);
        [DllImport("minCircleDll(c++).dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double getRadius(IntPtr cad);
        [DllImport("minCircleDll(c++).dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double getCenterX(IntPtr cad);
        [DllImport("minCircleDll(c++).dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double getCenterY(IntPtr cad);
        [DllImport("minCircleDll(c++).dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void findMinCircle(IntPtr cad, int attIdx, int corrIdx);
        /***Methods***/
        public void findAnomaly(string anomalyCSVPath, List<string> headersList)
        {
            this.CircleAnomalyDete = Create();
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
                }
            }
        }
        public Dictionary<string, List<int>> getAnomalyReport()
        {
            return AnomalyReport;
        }
        public void findMinCirc (int indexAtt , int indexCorr)
        {
            findMinCircle(CircleAnomalyDete, indexAtt , indexCorr);
        }
        public double getRadiu()
        {
            return getRadius(CircleAnomalyDete);
        }
        public double getX()
        {
            return getCenterX(CircleAnomalyDete);
        }
        public double getY()
        {
            return getCenterY(CircleAnomalyDete);
        }
    }
}
