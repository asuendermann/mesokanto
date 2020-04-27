using Serilog.Core;
using Serilog.Events;

namespace Commons.Configuration {
    public class ProjectNameEnricher : ILogEventEnricher {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) {
            var projectName = propertyFactory.CreateProperty("ProjectName", ConfigurationTk.InitialAssemblyName);
            logEvent.AddPropertyIfAbsent(projectName);
        }
    }
}