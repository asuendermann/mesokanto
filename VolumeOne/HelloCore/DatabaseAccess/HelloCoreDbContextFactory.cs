using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HelloCore.DatabaseAccess {
    public class HelloCoreDbContextFactory : IDesignTimeDbContextFactory<HelloCoreDbContext> {
        public HelloCoreDbContext CreateDbContext(string[] args) {
            var basePath = Directory.GetCurrentDirectory();
            Console.WriteLine(basePath);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var connectionString = configuration.GetConnectionString("LocalMesokantoDb");

            var optionsBuilder = new DbContextOptionsBuilder<HelloCoreDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new HelloCoreDbContext(optionsBuilder.Options);

        }
    }
}
