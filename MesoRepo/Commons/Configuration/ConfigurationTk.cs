using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Microsoft.Extensions.Configuration;

using Serilog;

namespace Commons.Configuration {
    public static class ConfigurationTk {
        public const string AspnetcoreEnvironment = "ASPNETCORE_ENVIRONMENT";

        public const string AppSettings = "appsettings";

        public const string SerilogSettings = "serilogsettings";

        public static readonly string Development = "Development";

        public static readonly string SectionApplicationSettings = "ApplicationSettings";

        public static readonly string ProjectConnectionString = "ProjectConnectionString";

        public static readonly string ProjectName = "ProjectName";

        static ConfigurationTk() {
            InitialAssemblyName = FindInitialAssemblyName;
        }

        public static string InitialAssemblyName { get; }

        private static string FindInitialAssemblyName {
            get {
                var frames = new StackTrace().GetFrames();
                var initialAssembly = (from f in frames
                        select f.GetMethod()?.ReflectedType?.Assembly.GetName().Name
                    ).Last();
                return initialAssembly;
            }
        }

        public static IConfigurationRoot ConfigureFromFile(string appSettings = AppSettings) {
            var currentDirectory = Directory.GetCurrentDirectory();
            var environment = Environment.GetEnvironmentVariable(AspnetcoreEnvironment) ?? Development;
            var configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile($"{appSettings}.json", false, true)
                .AddJsonFile($"{appSettings}.{environment}.json", true, true)
                .AddEnvironmentVariables()
                .Build();
            return configuration;
        }

        public static void ConfigureSerilogFromFile(string appSettings = AppSettings,
            string serilogSettings = SerilogSettings) {
            var currentDirectory = Directory.GetCurrentDirectory();
            var environment = Environment.GetEnvironmentVariable(AspnetcoreEnvironment) ?? "Development";
            var serilogConfiguration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile($"{appSettings}.json", false, true)
                .AddJsonFile($"{appSettings}.{environment}.json", true, true)
                .AddJsonFile($"{serilogSettings}.json", false, true)
                .AddJsonFile($"{serilogSettings}.{environment}.json", true, true)
                .AddEnvironmentVariables()
                .Build();
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.With(new ProjectNameEnricher())
                .ReadFrom.Configuration(serilogConfiguration);
            Log.Logger = loggerConfiguration.CreateLogger();
            Log.Information("Serilog Configuration from file succeeded");
        }

        /// <summary>
        ///     Extension method that reads the specified key from the specified section.
        /// </summary>
        /// <param name="configuration">the IConfiguration object.</param>
        /// <param name="section">the section where to reads the parameter value.</param>
        /// <param name="key">the key of the parameter value.</param>
        /// <param name="defaultValue">the default value for the property.</param>
        /// <returns>the value from the configuration if present, else the default value if specified, or finally an empty string.</returns>
        public static string GetSectionValue(this IConfiguration configuration, string section, string key,
            string defaultValue = null) {
            return configuration?.GetSection(section)[key] ?? defaultValue;
        }

        /// <summary>
        ///     Extension method that reads the specified key from the ApplicationSettings section.
        ///     Redirects to GetValue(section, key, defaultValue).
        /// </summary>
        /// <param name="configuration">the IConfiguration object.</param>
        /// <param name="key">the key of the parameter value.</param>
        /// <param name="defaultValue">the default value for the property.</param>
        /// <returns>the value from the configuration if present, else the default value if specified, or finally an empty string.</returns>
        public static string GetAppSetting(this IConfiguration configuration, string key, string defaultValue = null) {
            return configuration?.GetSectionValue(SectionApplicationSettings, key, defaultValue);
        }
    }
}