using System;
using System.Collections.Generic;
using System.Text;

using CommandLine;
using CommandLine.Text;

namespace HelloCore {
    public class HelloCoreOptions {
        public string ProgramName { get; set; }
        public string VersionInfo { get; protected set; }

/*
        public HelloCoreOptions(string programName, string versionInfo = null) {
            ProgramName = programName;
            VersionInfo = versionInfo;
        }

*/
        [Option('s', longName: "serilog", Default = "file",
            HelpText = "Specify how to configure Serilog logger.")]
        public string Serilog { get; set; }

    }
}
