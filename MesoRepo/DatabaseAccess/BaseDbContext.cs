using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Commons.Configuration;
using Commons.DomainModel.Base;

using DomainModel.Administration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using Serilog;

namespace DatabaseAccess {
    public class BaseDbContext : DbContext, IDesignTimeDbContextFactory<BaseDbContext> {
        public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options) {
            Requestor = typeof(BaseDbContext).Name;
        }

        public string Requestor { get; set; }

        public DbSet<Administrator> Administrators { get; set; }

        public DbSet<ProjectAdministrator> ProjectAdministrators { get; set; }

        public DbSet<MasterAdministrator> MasterAdministrators { get; set; }

        public BaseDbContext CreateDbContext(string[] args) {
            var configuration = ConfigurationTk.ConfigureFromFile();
            var connectionStringName = configuration.GetAppSetting(ConfigurationTk.ProjectConnectionString);
            var connectionString = configuration.GetConnectionString(connectionStringName);

            var optionsBuilder = new DbContextOptionsBuilder<BaseDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new BaseDbContext(optionsBuilder.Options);
        }

        public virtual void MarkEntries<T>(DateTime timeStamp, string requestor) {
            foreach (var entity in ChangeTracker.Entries<BaseAuditable<T>>()) {
                switch (entity.State) {
                    case EntityState.Modified:
                        entity.Entity.ModifiedAt = timeStamp;
                        entity.Entity.ModifiedBy = requestor;

                        break;
                    case EntityState.Added:
                        entity.Entity.CreatedAt = timeStamp;
                        entity.Entity.CreatedBy = requestor;
                        break;
                }
            }
        }

        /// <summary>
        ///     Validation should be done on each layer separately.
        ///     We perform an extensive validation to handle ValidationAttributes set outside
        ///     the context of EF and database. EF Core does not do this anymore (EF6 did).
        /// </summary>
        public virtual void ValidateChanges() {
            var errorMessages = new List<string>();
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)) {
                var entity = entry.Entity;
                var context = new ValidationContext(entity);
                var results = new List<ValidationResult>();

                if (Validator.TryValidateObject(entity, context, results, true) == false) {
                    foreach (var result in results) {
                        if (result != ValidationResult.Success) {
                            errorMessages.Add(result.ErrorMessage);
                            Log.Error(result.ErrorMessage);
                        }
                    }
                }
            }

            if (errorMessages.Any()) {
                throw new ValidationException(string.Join(';', errorMessages));
            }
        }

        /// <summary>
        ///     uses EF Fluent API to refine data model.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            if (null == modelBuilder) {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Administrator>().ToTable("Administrators")
                .HasDiscriminator<string>("Discriminator")
                .HasValue<ProjectAdministrator>(typeof(ProjectAdministrator).Name)
                .HasValue<MasterAdministrator>(typeof(MasterAdministrator).Name);

            modelBuilder.Entity<Administrator>().HasKey(p => p.Id);
            modelBuilder.Entity<Administrator>()
                .HasIndex(w => w.UserIdentityName).IsUnique();
        }

        public override int SaveChanges() {
            var now = DateTime.UtcNow;
            var requestor = Requestor;
            MarkEntries<int>(now, requestor);

            ValidateChanges();

            return base.SaveChanges();
        }

        public virtual void DetachAllEntities() {
            foreach (var entry in ChangeTracker.Entries()) {
                entry.State = EntityState.Detached;
            }
        }
    }
}