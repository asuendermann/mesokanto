using System;

using HelloCoreCommons.Configuration;

using HelloCoreDal.DataAccessLayer;
using HelloCoreDal.DomainModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NCoreTest {
    public static class DependencyInjector {
        public static IServiceProvider GetServiceProvider() {
            var services = new ServiceCollection();

            services.AddScoped<IConfigurationService, ConfigurationService>();

            // Tell Services how to make a DbContext
            services.AddScoped(provider => {
                var configurationService = provider.GetService<IConfigurationService>();
                var optionsBuilder =
                    new DbContextOptionsBuilder<DemoDbContext>()
                        .UseSqlServer(configurationService.ProjectConnectionString);
                return new DemoDbContext(optionsBuilder.Options);
            });

/*
            services.AddScoped<IGenericRepository<Administrator, int>>(provider => {
                var dbContext = provider.GetService(typeof(DemoDbContext)) as DemoDbContext;
                return new GenericDbRepository<Administrator, int>(dbContext);
            });

            services.AddScoped<IGenericRepository<MasterAdministrator, int>>(provider => {
                var dbContext = provider.GetService(typeof(DemoDbContext)) as DemoDbContext;
                return new GenericDbRepository<MasterAdministrator, int>(dbContext);
            });

            services.AddScoped<IGenericRepository<ProjectAdministrator, int>>(provider => {
                var dbContext = provider.GetService(typeof(DemoDbContext)) as DemoDbContext;
                return new GenericDbRepository<ProjectAdministrator, int>(dbContext);
            });
*/


            return services.BuildServiceProvider();
        }
    }
}