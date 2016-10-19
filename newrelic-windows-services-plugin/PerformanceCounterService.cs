using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Diagnostics;
using NewRelic.Platform.Sdk.Utils;

namespace newrelic_windows_services_plugin
{
    class PerformanceCounterService
    {
        private Logger log = Logger.GetLogger(typeof(PerformanceCounterService).Name);
        private List<KeyValuePair<string, PerformanceCounter>> counters;
        private ServiceController controller;
        private string machineName = String.Empty;
        private string serviceName = String.Empty;
        private string instanceName = String.Empty;
        private int processId = 0;
        
        public PerformanceCounterService(string machineName, string serviceName) {
            if (String.IsNullOrWhiteSpace(machineName))
            {
                this.machineName = machineName;
            }
            else
            {
                this.machineName = machineName;
            }
            this.serviceName = serviceName;
            this.counters = new List<KeyValuePair<string, PerformanceCounter>>();
            this.controller = new ServiceController();
        }

        public void initPerformanceCounters()
        {
            try
            {
                controller = new ServiceController(this.serviceName, this.machineName);
                SafeHandle handle = controller.ServiceHandle;
                processId = UNMANAGED.findProcessId(handle);
                instanceName = getProcessNameById(processId);

                PerformanceCounter cpuUtilization = new PerformanceCounter();
                cpuUtilization.CategoryName = "Process";
                cpuUtilization.CounterName = "% Processor Time";
                cpuUtilization.InstanceName = instanceName;
                cpuUtilization.MachineName = this.machineName;
                counters.Add(new KeyValuePair<string, PerformanceCounter>(nameof(cpuUtilization), cpuUtilization));

                PerformanceCounter memoryUtilization = new PerformanceCounter();
                memoryUtilization.CategoryName = "Process";
                memoryUtilization.CounterName = "Working Set";
                memoryUtilization.InstanceName = instanceName;
                memoryUtilization.MachineName = this.machineName;
                counters.Add(new KeyValuePair<string, PerformanceCounter>(nameof(memoryUtilization), memoryUtilization));

                PerformanceCounter ioUtilization = new PerformanceCounter();
                ioUtilization.CategoryName = "Process";
                ioUtilization.CounterName = "IO Data Bytes/sec";
                ioUtilization.InstanceName = instanceName;
                ioUtilization.MachineName = this.machineName;
                counters.Add(new KeyValuePair<string, PerformanceCounter>(nameof(ioUtilization), ioUtilization));


                PerformanceCounter diskUtilization = new PerformanceCounter();
                diskUtilization.CategoryName = "PhysicalDisk";
                diskUtilization.CounterName = "Disk Transfers/sec";
                diskUtilization.InstanceName = "_Total";
                diskUtilization.MachineName = this.machineName;
                counters.Add(new KeyValuePair<string, PerformanceCounter>(nameof(diskUtilization), diskUtilization));

                //PerformanceCounter networdUtilization = new PerformanceCounter();
                //networdUtilization.CategoryName = "Network Interface";
                //networdUtilization.CounterName = "Bytes Total/sec";
                //networdUtilization.InstanceName = "Local Area Connection* 4";
                //networdUtilization.MachineName = this.machineName;
                //counters.Add(new KeyValuePair<string, PerformanceCounter>(nameof(networdUtilization), networdUtilization));

            }
            catch (Exception ex)
            {
                log.Error("Failed to initialize the performance counters for machine: {0} service: {1}", machineName, serviceName);
                log.Error(ex.Message);
            }
        }

        public ServiceCounters getProcessDetails()
        {
            try
            {
                ServiceCounters details = new ServiceCounters();
                details.status = (int) controller.Status;
                details.name = instanceName;
                details.processId = processId;

                details.performanceValues = new List<KeyValuePair<string, float>>();
                foreach (var item in counters)
                {
                    details.performanceValues.Add(new KeyValuePair<string, float>(item.Key, item.Value.NextValue()));
                }
                return details;
            }
            catch (Exception ex)
            {
                log.Error("Error getting performance data for machine: {0}, service: {1}", this.machineName, this.serviceName);
                log.Error(ex.Message);
                return null;
            }
        }

        private string getProcessNameById(int processId)
        {
            Process process = Process.GetProcessById(processId);
            string processName = process.ProcessName;
            if (!string.IsNullOrEmpty(processName)) {
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes.Length > 0) {
                    for (int i = 0; i < processes.Length; i++)
                    {
                        string tempProcessName = i == 0 ? processName : processName + "#" + i.ToString();
                        if (PerformanceCounterCategory.CounterExists("ID Process", "Process", this.machineName)) {
                            PerformanceCounter idCounter = new PerformanceCounter("Process", "ID Process", tempProcessName, this.machineName);
                            if (processId == idCounter.RawValue) {
                                return tempProcessName;
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}