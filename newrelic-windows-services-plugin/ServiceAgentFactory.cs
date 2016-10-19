using System;
using System.Collections.Generic;
using NewRelic.Platform.Sdk;

namespace newrelic_windows_services_plugin
{
    class ServiceAgentFactory : AgentFactory
    {
        public override Agent CreateAgentWithConfiguration(IDictionary<string, object> properties)
        {
            string name = (string)properties["name"];
            string machine = (string)properties["machineName"];
            string service = (string)properties["serviceName"];

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(machine) || string.IsNullOrEmpty(service))
            {
                throw new ArgumentNullException("'name', 'machine name' and 'service service name' may not be empty. Please check your plugin.json configuration file.");
            }
            return new ServiceAgent(name, machine, service);
            
        }
    }
}
