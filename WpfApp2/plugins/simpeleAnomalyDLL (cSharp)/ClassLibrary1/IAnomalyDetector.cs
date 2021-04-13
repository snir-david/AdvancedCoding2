using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopFGApp
{
    interface IAnomalyDetector
    {
        void findAnomaly(string anomalyCSVPath, List<string> headersList);
        Dictionary<string, List<int>> getAnomalyReport();
    }
}
