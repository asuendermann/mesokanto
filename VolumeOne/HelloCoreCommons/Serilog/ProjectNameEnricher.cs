using System.Diagnostics;
using System.Linq;

using Serilog.Core;
using Serilog.Events;

namespace HelloCoreCommons.Serilog {
    public class ProjectNameEnricher : ILogEventEnricher {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) {
            var frames = new StackTrace().GetFrames();
            var initialAssembly = (from f in frames
                    select f.GetMethod()?.ReflectedType?.Assembly.GetName().Name
                ).Distinct().Last();
            var projectName = propertyFactory.CreateProperty("ProjectName", initialAssembly);
            logEvent.AddPropertyIfAbsent(projectName);
        }
    }
}