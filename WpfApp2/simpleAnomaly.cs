using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DesktopFGApp
{
    class simpleAnomaly
    {
        public static Dictionary<string, List<float>> AnomalyReport = new Dictionary<string, List<float>>();

        // need to insert user input for dll filr
        [DllImport("C:\\Users\\snira\\source\\repos\\snir-david\\DesktopFGApp\\WpfApp2\\plugins\\timeseriesDLL\\Debug\\timeseriesDLL.dll",CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Create();
        [DllImport("C:\\Users\\snira\\source\\repos\\snir-david\\DesktopFGApp\\WpfApp2\\plugins\\timeseriesDLL\\Debug\\timeseriesDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void learnnig(IntPtr sad, string CSVfileName);
        [DllImport("C:\\Users\\snira\\source\\repos\\snir-david\\DesktopFGApp\\WpfApp2\\plugins\\timeseriesDLL\\Debug\\timeseriesDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void detecting(IntPtr sad, IntPtr wrapAR, string detectfileName);
        [DllImport("C:\\Users\\snira\\source\\repos\\snir-david\\DesktopFGApp\\WpfApp2\\plugins\\timeseriesDLL\\Debug\\timeseriesDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateVectorWrapper();
        [DllImport("C:\\Users\\snira\\source\\repos\\snir-david\\DesktopFGApp\\WpfApp2\\plugins\\timeseriesDLL\\Debug\\timeseriesDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern long getTS(IntPtr vec, int x);
        [DllImport("C:\\Users\\snira\\source\\repos\\snir-david\\DesktopFGApp\\WpfApp2\\plugins\\timeseriesDLL\\Debug\\timeseriesDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getDP(IntPtr vec, int x);
        [DllImport("C:\\Users\\snira\\source\\repos\\snir-david\\DesktopFGApp\\WpfApp2\\plugins\\timeseriesDLL\\Debug\\timeseriesDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getDPLen(IntPtr vec, int x);
        [DllImport("C:\\Users\\snira\\source\\repos\\snir-david\\DesktopFGApp\\WpfApp2\\plugins\\timeseriesDLL\\Debug\\timeseriesDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int vectorSize(IntPtr vec);
        

            public static void dllrunner()
        {
            IntPtr a = Create();
            learnnig(a, "C:\\Users\\snira\\source\\repos\\snir-david\\DesktopFGApp\\WpfApp2\\reg_flight.csv");
            IntPtr vec = CreateVectorWrapper();
            detecting(a, vec, "C:\\Users\\snira\\source\\repos\\snir-david\\DesktopFGApp\\WpfApp2\\anomaly_flight.csv");
            int vs = vectorSize(vec);
            for (int i =0; i < vs; i++ )
            {
                long val = getTS(vec, i);

                IntPtr desc = getDP(vec, i);
                /*
                if (AnomalyReport.ContainsKey(desc))
                {
                    AnomalyReport[desc].Add(val);
                } else
                {
                    AnomalyReport.Add(desc, new List<float>());
                    AnomalyReport[desc].Add(val);
                }*/
            } 
        }
    }
}
