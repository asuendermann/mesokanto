using Commons.Configuration;
using DatabaseAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace UnitTests {
    public static class DependencyInjector {
        public static IServiceProvider GetServiceProvider() {
            var services = new ServiceCollection();

            services.AddScoped<IConfigurationService, ConfigurationService>();

            services.AddScoped(provider => {
                var configurationService = provider.GetService<IConfigurationService>();
                var optionsBuilder =
                    new DbContextOptionsBuilder<BaseDbContext>()
                        .UseSqlServer(configurationService.ProjectConnectionString);
                return new BaseDbContext(optionsBuilder.Options);
            });


            return services.BuildServiceProvider();
        }
    }
}
