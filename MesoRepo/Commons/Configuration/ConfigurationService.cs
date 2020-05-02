using System;
using System.IO;
using Microsoft.Extensions.Configuration;

using static Commons.Configuration.ConfigurationExtensions;

namespace Commons.Configuration {
    public class ConfigurationService : IConfigurationService {

        public ConfigurationService(string appSettings = AppSettings) {
            var currentDirectory = Directory.GetCurrentDirectory();
            var environment = Environment.GetEnvironmentVariable(AspnetcoreEnvironment) ?? "Development";
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
                var connectionStringName = Configuration.GetAppSetting(KeyProjectConnectionString);
                var connectionString = Configuration.GetConnectionString(connectionStringName);
                return connectionString;
            }
        }
    }
}