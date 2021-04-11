using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopFGApp
{
    interface IAnomalyDetector
    {
        void findAnomaly();
        Dictionary<string, List<float>> getAnomalyReport();
    }
}
