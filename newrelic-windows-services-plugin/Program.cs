using System;
using NewRelic.Platform.Sdk;
using NewRelic.Platform.Sdk.Utils;

namespace newrelic_windows_services_plugin
{
    class Program
    {
        private static Logger log = Logger.GetLogger(typeof(Program).Name);
        static void Main(string[] args)
        {
            try
            {
                Runner runner = new Runner();
                runner.Add(new ServiceAgentFactory());
                runner.SetupAndRun();
            }
            catch (Exception ex)
            {
                log.Error("Failed to start the windows services plugin. " + ex.Message);
                return;
            }
        }
    }
}
