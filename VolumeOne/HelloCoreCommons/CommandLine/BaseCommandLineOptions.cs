using System;
using System.Collections.Generic;
using System.Text;

using CommandLine;

namespace HelloCoreCommons.CommandLine {
    public class BaseCommandLineOptions {
        [Option('h', "Hold", Required = false,
            HelpText = "Set program on hold on exit. Program will only terminate if <CR> is hit.")]
        public bool Hold { get; set; }
    }
}
