using System.IO;

using HelloCoreAdminMvc.Resources;
using HelloCoreAdminMvc.WindowsAuthorization;

using HelloCoreBll.BusinesLayerLogic;

using HelloCoreCommons.Configuration;

using HelloCoreDal.DataAccessLayer;
using HelloCoreDal.DomainModel;
using HelloCoreDal.Repository;

using HelloCoreVm;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace HelloCoreAdminMvc {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddAuthentication(options => {
                options.DefaultChallengeScheme = WindowsAuthenticationSchemeHandler.SchemaName;

                // you can also skip this to make the challenge scheme handle the forbid as well
                options.DefaultForbidScheme = WindowsAuthenticationSchemeHandler.SchemaName;

                // of course you also need to register that scheme, e.g. using
                options.AddScheme<WindowsAuthenticationSchemeHandler>
                (WindowsAuthenticationSchemeHandler.SchemaName,
                    WindowsAuthenticationSchemeHandler.SchemaName);
            });

            services.AddScoped<IWindowsAuthorizationService, WindowsAuthorizationService>();
            services.AddScoped<IAuthorizationHandler, IsWindowsAdminHandler>();
            services.AddScoped<IAuthorizationHandler, IsWindowsMasterHandler>();
            services.AddAuthorization(options => {
                options.AddPolicy(IsWindowsAdminRequirement.IsWindowsAdminRequirementPolicy,
                    policy => { policy.Requirements.Add(new IsWindowsAdminRequirement()); });
                options.AddPolicy(IsWindowsMasterRequirement.IsWindowsMasterRequirementPolicy,
                    policy => { policy.Requirements.Add(new IsWindowsMasterRequirement()); });
            });

            services.AddScoped<IConfigurationService, ConfigurationService>();

            // Tell Services how to make a DbContext
            services.AddScoped(provider => {
                var configurationService = provider.GetService<IConfigurationService>();
                var optionsBuilder =
                    new DbContextOptionsBuilder<DemoDbContext>()
                        .UseSqlServer(configurationService.ProjectConnectionString);
                return new DemoDbContext(optionsBuilder.Options);
            });

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

            services.AddScoped<IAdministratorsBllManager, AdministratorsBllManager>();

            services.AddScoped<IViewModelManager, ViewModelManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            ConfigurationTk.ConfigureSerilogFromFile();
            Log.Information(AdminResources.Message_Startup);

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}