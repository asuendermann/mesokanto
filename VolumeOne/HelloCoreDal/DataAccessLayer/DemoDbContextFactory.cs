using System;
using System.IO;

using HelloCoreCommons.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HelloCoreDal.DataAccessLayer {
    /// <summary>
    ///     see https://docs.microsoft.com/de-de/ef/core/miscellaneous/cli/dbcontext-creation
    ///     Working with Migrations requires this Factory implementation to be present.
    /// </summary>
    public class DemoDbContextFactory : IDesignTimeDbContextFactory<DemoDbContext> {
        public DemoDbContext CreateDbContext(string[] args) {
            var environment = Environment.GetEnvironmentVariable(ConfigurationTk.AspnetcoreEnvironment) ??
                              "Development";
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{ConfigurationTk.AppSettings}.json", false, true)
                .AddJsonFile($"{ConfigurationTk.AppSettings}.{environment}.json", false, true)
                .Build();

            return CreateDbContext(args, configuration);
        }

        public DemoDbContext CreateDbContext(string[] args, IConfigurationRoot configuration) {
            var basePath = Directory.GetCurrentDirectory();
            Console.WriteLine(basePath);

            var connectionStringName = configuration.GetAppSetting(ConfigurationTk.ProjectConnectionString);
            var connectionString = configuration.GetConnectionString(connectionStringName);

            var optionsBuilder = new DbContextOptionsBuilder<DemoDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new DemoDbContext(optionsBuilder.Options);
        }
    }
}