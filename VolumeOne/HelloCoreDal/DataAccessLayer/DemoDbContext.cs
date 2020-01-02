using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using HelloCoreCommons.Configuration;

using HelloCoreDal.DomainModel;

using Microsoft.EntityFrameworkCore;

using Serilog;

namespace HelloCoreDal.DataAccessLayer {
    public class DemoDbContext : DbContext {
        public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options) {
        }

        public DbSet<Administrator> Administrators { get; set; }

        public DbSet<ProjectAdministrator> ProjectAdministrators { get; set; }

        public DbSet<MasterAdministrator> MasterAdministrators { get; set; }

        public override int SaveChanges() {
            var now = DateTime.UtcNow;
            var requestor = ConfigurationTk.InitialAssemblyName;
            MarkEntries<int>(now, requestor);

            ValidateChanges();

            return base.SaveChanges();
        }

        /// <summary>
        ///     Validation should be done on each layer separately.
        ///     We perform an extensive validation to handle ValidationAttributes set outside
        ///     the context of EF and database. EF Core does not do this anymore (EF6 did).
        /// </summary>
        public void ValidateChanges() {
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

        public void MarkEntries<T>(DateTime timeStamp, string requestor) {
            foreach (var entity in ChangeTracker.Entries<AbstractAuditableBase<T>>()) {
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

        public void DetachAllEntities() {
            var changedEntriesCopy = ChangeTracker.Entries()

                //.Where(e => e.State == EntityState.Added ||
                //            e.State == EntityState.Modified ||
                //            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy) {
                entry.State = EntityState.Detached;
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

            // -------------------------------
            // Configure ProjectAdministrators
            // -------------------------------
            modelBuilder.Entity<Administrator>().ToTable("Administrators")
                .HasDiscriminator<string>("Discriminator")
                .HasValue<ProjectAdministrator>(typeof(ProjectAdministrator).Name)
                .HasValue<MasterAdministrator>(typeof(MasterAdministrator).Name);

            modelBuilder.Entity<Administrator>().HasKey(p => p.Id);
            modelBuilder.Entity<Administrator>()
                .HasIndex(w => w.UserIdentityName).IsUnique();
        }

        public void SeedDatabase(string masterUserIdentityName) {
            var master = new MasterAdministrator {
                UserIdentityName = @"masterUserIdentityName",
                Name = "Master User",
                Email = "master@domain.com",
                Phone = "1234"
            };

            Administrators.Add(master);

            SaveChanges();
        }
    }
}