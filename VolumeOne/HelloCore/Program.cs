using System;
using System.IO;

using HelloCore.SerilogExtensions;

using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Debugging;
using Serilog.Events;

namespace HelloCore {
    internal class Program {
        public static readonly string AspnetcoreEnvironment = "ASPNETCORE_ENVIRONMENT";

        public static readonly string AppSettings = "appsettings";

        public static readonly string SerilogSettings = "serilogsettings";

        private static readonly string ConnectionString =
            "Data Source=(localdb)\\MSSQLLocalDb;Initial Catalog=MesokantoDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public static readonly string TableSerilog = "SeriLog";

        public static void Main(string[] args) {
            try {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile(
                        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                        true)
                    .Build();

                //var context = new HelloCoreDbContextFactory().CreateDbContext(args);
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();
                //Console.WriteLine("DB Setup Complete");

                SelfLog.Enable(Console.Error);

                ConfigureSerilogLocally();
                //ConfigureSerilogFromFile();

                Log.Verbose("Hello Core! logs Verbose messages");
                Log.Debug("Hello Core! logs Debug messages");
                Log.Information("Hello Core! logs Information messages");
                Log.Warning("Hello Core! logs Warning messages");
                Log.Error("Hello Core! logs Error messages");
                Log.Fatal("Hello Core! logs Fatal messages");
            } catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                Log.Fatal(ex, "DB Setup terminated with exception");
            } finally {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureSerilogLocally() {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json",
                    true)
                .Build();
            var connectionString = configuration.GetConnectionString("LocalMesokantoDb");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Console(LogEventLevel.Verbose, theme: CustomConsoleTheme.KunterBunt)
                .WriteTo.File("C:\\tmp\\VolumeOne\\Logs\\HelloCore.txt", LogEventLevel.Information,
                    rollingInterval: RollingInterval.Day)
                .WriteTo.MSSqlServer(connectionString, TableSerilog,restrictedToMinimumLevel: LogEventLevel.Verbose, autoCreateSqlTable: true)
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