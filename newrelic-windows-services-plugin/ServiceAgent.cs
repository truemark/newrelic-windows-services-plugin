using System;
using NewRelic.Platform.Sdk;
using NewRelic.Platform.Sdk.Utils;

namespace newrelic_windows_services_plugin
{
    class ServiceAgent : Agent
    {
        private Logger log = Logger.GetLogger(typeof(ServiceAgent).Name);
        public override string Guid { get; } = "com.truemarkit.newrelic.windows-service";

        public override string Version { get; } = "1.0.0";

        private string name;
        private string machineName;
        private string serviceName;

        public ServiceAgent(string name, string machineName, string serviceName)
        {
            this.name = name;
            this.machineName = machineName;
            this.serviceName = serviceName;
        }

        public override string GetAgentName()
        {
            return this.name;
        }

        public override void PollCycle()
        {
            reportServiceUtilizationData();
        }

        private void reportServiceUtilizationData()
        {
            log.Debug("Fetching performance data for machine: {0}, service: {1}", this.machineName, this.serviceName);
            PerformanceCounterService service = new PerformanceCounterService(machineName, serviceName);
            service.initPerformanceCounters();
            ServiceCounters counter = service.getProcessDetails();
            if (counter == null)
            {
                log.Error("Error gettting performace data for service: {0}, Null counter returned.", this.serviceName);
            }
            ReportMetric(this.machineName + "/" + this.serviceName + "/status", "#", counter.status);
            foreach (var pair in counter.performanceValues)
            {
                string metricName = this.machineName + "/" + this.serviceName + "/" + pair.Key;
                ReportMetric(metricName, "#", pair.Value);
            }
        }
    }
}
