using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using HelloCore.SerilogExtensions;

using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

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
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile($"appsettings.{environment}.json", true)
                    .Build();

                //var context = new HelloCoreDbContextFactory().CreateDbContext(args);
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();
                //Console.WriteLine("DB Setup Complete");

                SelfLog.Enable(Console.Error);

                //ConfigureSerilogLocally();
                ConfigureSerilogFromFile();

                Log.Verbose(HelloCoreResources.Message_Welcome, LogEventLevel.Verbose, environment);
                Log.Debug(HelloCoreResources.Message_Welcome, LogEventLevel.Debug, environment);
                Log.Information(HelloCoreResources.Message_Welcome, LogEventLevel.Information, environment);
                Log.Warning(HelloCoreResources.Message_Welcome, LogEventLevel.Warning, environment);
                Log.Error(HelloCoreResources.Message_Welcome, LogEventLevel.Error, environment);
                Log.Fatal(HelloCoreResources.Message_Welcome, LogEventLevel.Verbose, environment);
            } catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                var innerEx = ex.InnerException;

                while (null != innerEx) {
                    Console.WriteLine(innerEx.StackTrace);
                    innerEx = innerEx.InnerException;
                }
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

            var columnOptions = new ColumnOptions {
                AdditionalColumns = new List<SqlColumn> {
                    new SqlColumn {
                        ColumnName = "MachineName",
                        DataType = SqlDbType.NVarChar,
                        DataLength = 64
                    },
                    new SqlColumn {
                        ColumnName = "ThreadId",
                        DataType = SqlDbType.Int
                    },
                    new SqlColumn {
                        ColumnName = "ProjectName",
                        DataType = SqlDbType.NVarChar,
                        DataLength = 128
                    }
                }
            };

            // Configure LogEvent column
            columnOptions.Store.Add(StandardColumn.LogEvent);
            columnOptions.LogEvent.ExcludeAdditionalProperties = true;
            columnOptions.LogEvent.ExcludeStandardColumns = true;

            // Remove unused Standard Columns
            columnOptions.Store.Remove(StandardColumn.MessageTemplate);
            columnOptions.Store.Remove(StandardColumn.Properties);

            // Reconfigure Timestamp column
            columnOptions.TimeStamp.ColumnName = "TimestampUTC";
            columnOptions.TimeStamp.ConvertToUtc = true;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.With(new ProjectNameEnricher())
                .WriteTo.Console(LogEventLevel.Verbose, theme: CustomConsoleTheme.KunterBunt)
                .WriteTo.File("C:\\tmp\\VolumeOne\\Logs\\HelloCore.txt", 
                    LogEventLevel.Information,
                    rollingInterval: RollingInterval.Day)
                .WriteTo.MSSqlServer(connectionString, TableSerilog,
                    restrictedToMinimumLevel: LogEventLevel.Verbose, 
                    autoCreateSqlTable: true, columnOptions: columnOptions)
                .CreateLogger();
            Log.Information("Serilog Configuration locally succeeded");
        }

        private static void ConfigureSerilogFromFile() {
            var serilogConfiguration = ReadSerilogConfiguration();
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.With(new ProjectNameEnricher())
                .ReadFrom.Configuration(serilogConfiguration);
            Log.Logger = loggerConfiguration.CreateLogger();
            Log.Information("Serilog Configuration from file succeeded");
        }

        private static IConfigurationRoot ReadSerilogConfiguration() {
            var currentDirectory = Directory.GetCurrentDirectory();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile($"appsettings.json", false, true)
                .AddJsonFile($"{SerilogSettings}.json", false, true)
                .AddJsonFile($"{SerilogSettings}.{environment}.json", false, true)
                .AddEnvironmentVariables()
                .Build();
            return configuration;
        }
    }
}