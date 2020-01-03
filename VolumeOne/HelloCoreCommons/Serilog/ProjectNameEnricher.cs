using HelloCoreCommons.Configuration;

using Serilog.Core;
using Serilog.Events;

namespace HelloCoreCommons.Serilog {
    public class ProjectNameEnricher : ILogEventEnricher {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) {
            var projectName = propertyFactory.CreateProperty("ProjectName", ConfigurationTk.InitialAssemblyName);
            logEvent.AddPropertyIfAbsent(projectName);
        }
    }
}