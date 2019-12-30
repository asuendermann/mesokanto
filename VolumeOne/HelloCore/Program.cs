using System;
using System.IO;

using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Events;

namespace HelloCore {
    internal class Program {
        public static string AspnetcoreEnvironment = "ASPNETCORE_ENVIRONMENT";

        public static string AppSettings = "appsettings";
        public static string SerilogSettings = "serilogsettings";

        private static void Main(string[] args) {

            Serilog.Debugging.SelfLog.Enable(Console.Error);

            ConfigureSerilogLocally();
            //ConfigureSerilogFromFile();

            Log.Verbose("Hello Core! logs Verbose messages");
            Log.Debug("Hello Core! logs Debug messages");
            Log.Information("Hello Core! logs Information messages");
            Log.Warning("Hello Core! logs Warning messages");
            Log.Error("Hello Core! logs Error messages");
            Log.Fatal("Hello Core! logs Fatal messages");
        }

        private static void ConfigureSerilogLocally() {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console( restrictedToMinimumLevel: LogEventLevel.Verbose)
                .WriteTo.File("C:\\tmp\\VolumeOne\\Logs\\HelloCore.txt", restrictedToMinimumLevel: LogEventLevel.Information, rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Information("Serilog Configuration locally succeeded");
        }

        private static void ConfigureSerilogFromFile() {
            var serilogConfiguration = ReadSerilogConfiguration();
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(serilogConfiguration);
            Log.Logger = loggerConfiguration.CreateLogger();
            Log.Information("Serilog Configuration from file succeeded");
        }

        private static IConfigurationRoot ReadSerilogConfiguration() {
            var currentDirectory = Directory.GetCurrentDirectory();
            var environmentName = Environment.GetEnvironmentVariable(AspnetcoreEnvironment);
            var configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile($"{AppSettings}.json", false, true)
                .AddJsonFile($"{AppSettings}.{environmentName}.json", true)
                .AddJsonFile($"{SerilogSettings}.json", false, true)
                .AddJsonFile($"{SerilogSettings}.{environmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();
            return configuration;
        }
    }
}