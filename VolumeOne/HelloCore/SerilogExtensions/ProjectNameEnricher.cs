using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.Extensions.Configuration;

using Serilog.Core;
using Serilog.Events;

namespace HelloCore.SerilogExtensions {
    public class ProjectNameEnricher : ILogEventEnricher {

        public ProjectNameEnricher(IConfigurationRoot configuration) {
            var frames = new StackTrace().GetFrames();
            var initialAssembly = (from f in frames
                    select f.GetMethod()?.ReflectedType?.Assembly.GetName().Name
                ).Distinct().Last();
            projectName = initialAssembly;
        }

        private readonly string projectName;

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) {
            var projectName = propertyFactory.CreateProperty("ProjectName", this.projectName);
            logEvent.AddPropertyIfAbsent(projectName);

        }
    }
}
