using CommandLine;

using HelloCoreCommons.CommandLine;

namespace HelloCoreDal {
    public class DbContextArgs : BaseCommandLineOptions {

        [Option('d', longName: "DeleteIfExists", Required = false, Default = false,
            HelpText = "Delete existing database before running the setup program.")]
        public bool DeleteIfExists { get; set; }

        [Option('m', "MasterUser", Required = true,
            HelpText = "Specify the first Master Windows User Identity (with Domain name).")]
        public string MasterUserIdentity { get; set; }

    }
}