using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopFGApp
{
    interface IAnomalyDetector
    {
        void findAnomaly(string anomalyCSVPath);
        Dictionary<string, List<float>> getAnomalyReport();
    }
}
