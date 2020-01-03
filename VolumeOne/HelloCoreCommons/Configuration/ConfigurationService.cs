using System;
using System.IO;

using Microsoft.Extensions.Configuration;

namespace HelloCoreCommons.Configuration {
    public class ConfigurationService : IConfigurationService {
        public ConfigurationService(string appSettings = ConfigurationTk.AppSettings) {
            var currentDirectory = Directory.GetCurrentDirectory();
            var environmentName = Environment.GetEnvironmentVariable(ConfigurationTk.AspnetcoreEnvironment);
            var environment = Environment.GetEnvironmentVariable(ConfigurationTk.AspnetcoreEnvironment) ?? "Development";
            Configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile($"{appSettings}.json", false, true)
                .AddJsonFile($"{appSettings}.{environment}.json", true, true)
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfigurationRoot Configuration { get; }

        public string GetAppSetting(string key, string defaultValue = null) {
            return Configuration.GetAppSetting(key, defaultValue);
        }

        public string GetSectionValue(string section, string key, string defaultValue = null) {
            return Configuration.GetSection(section)[key] ?? defaultValue;
        }

        public string ProjectConnectionString {
            get {
                var connectionStringName = Configuration.GetAppSetting(ConfigurationTk.ProjectConnectionString);
                var connectionString = Configuration.GetConnectionString(connectionStringName);
                return connectionString;
            }
        }
    }
}