using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newrelic_windows_services_plugin
{
    class ServiceCounters
    {
        public int status { get; set; }
        public int processId { get; set; }
        public string name { get; set; }
        public List<KeyValuePair<string, float>> performanceValues;
    }
}
