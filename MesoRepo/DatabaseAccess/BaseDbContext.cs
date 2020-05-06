
using Commons.Configuration;

using DomainModel.Administration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using System;

namespace DatabaseAccess {
    public class BaseDbContext : DbContext, IDesignTimeDbContextFactory<BaseDbContext> {

        public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options) {
        }

        public BaseDbContext CreateDbContext(string[] args) {
            var configuration = Commons.Configuration.ConfigurationExtensions.ConfigureFromFile();
            var connectionStringName = configuration.GetAppSetting(Commons.Configuration.ConfigurationExtensions.KeyProjectConnectionString);
            var connectionString = configuration.GetConnectionString(connectionStringName);

            var optionsBuilder = new DbContextOptionsBuilder<BaseDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new BaseDbContext(optionsBuilder.Options);
        }

        /// <summary>
        ///     uses EF Fluent API to define data model.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            if (null == modelBuilder) {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Administrator>().ToTable("Administrators")
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Administrator>(nameof(Administrator))
                .HasValue<MasterAdministrator>(nameof(MasterAdministrator));

            modelBuilder.Entity<Administrator>().HasKey(p => p.Id);
            modelBuilder.Entity<Administrator>()
                .HasIndex(w => w.UserIdentityName).IsUnique();

            modelBuilder.Entity<Project>().HasKey(p => p.Id);
            modelBuilder.Entity<Project>()
                .HasIndex(w => w.Name).IsUnique();

            modelBuilder.Entity<ProjectAdministrator>().HasKey(p => p.Id);
            modelBuilder.Entity<ProjectAdministrator>().HasOne<Administrator>(nameof(Administrator))
                .WithMany(l => l.AdministratorProjects)
                .HasForeignKey(p => p.AdministratorId)
                .IsRequired();
            modelBuilder.Entity<ProjectAdministrator>().HasOne<Project>(nameof(Project))
                .WithMany(l => l.ProjectAdministrators)
                .HasForeignKey(p => p.ProjectId)
                .IsRequired();
        }

        public override int SaveChanges() {
            this.MarkEntries<int>();
            this.ValidateChanges();

            return base.SaveChanges();
        }
    }
}