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
        // need to insert user input for dll filr
        [DllImport("C:\\Users\\tallor\\source\\repos\\snir-david\\DesktopFGApp\\WpfApp2\\plugins\\timeseriesDLL\\Debug\\timeseriesDLL.dll")]
        public static extern IntPtr Create();
        [DllImport("C:\\Users\\tallor\\source\\repos\\snir-david\\DesktopFGApp\\WpfApp2\\plugins\\timeseriesDLL\\Debug\\timeseriesDLL.dll")]
        public static extern void learnnig(IntPtr sad, string CSVfileName);


        public static void dllrunner()
        {
            IntPtr a = Create();
            Console.WriteLine("Hello World!");
           // learnnig(a,"reg_flight.csv");
            Console.WriteLine("Hello World22222!");
        }
    }
}
