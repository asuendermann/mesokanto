using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;

namespace HelloCore {
     class Program {

        public static string AspnetcoreEnvironment = "ASPNETCORE_ENVIRONMENT";
        public static string SerilogSettings = "serilogsettings";

        static void Main(string[] args) {
            ConfigureSerilogLocally();
            //ConfigureSerilogFromFile();
            Log.Information("Hello Core!");
        }


        private static void ConfigureSerilogLocally() {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();
        }

        private static void ConfigureSerilogFromFile() {
            var serilogConfiguration = ReadSerilogConfiguration();
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(serilogConfiguration);
            Log.Logger = loggerConfiguration.CreateLogger();
        }

        private static IConfigurationRoot ReadSerilogConfiguration() {
            var currentDirectory = Directory.GetCurrentDirectory();
            var environmentName = Environment.GetEnvironmentVariable(AspnetcoreEnvironment);
            var configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile($"{SerilogSettings}.json", false, true)
                .AddJsonFile($"{SerilogSettings}.{environmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();
            return configuration;
        }
    }
}
