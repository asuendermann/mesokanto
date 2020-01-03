using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using HelloCoreCommons.Configuration;
using HelloCoreCommons.DomainModel;

using Microsoft.EntityFrameworkCore;

using Serilog;

namespace HelloCoreDal.DataAccessLayer {
    public class BaseDbContext : DbContext {
        public BaseDbContext(DbContextOptions<DemoDbContext> options) : base(options) {
        }

        public override int SaveChanges() {
            var now = DateTime.UtcNow;
            var requestor = ConfigurationTk.InitialAssemblyName;
            MarkEntries<int>(now, requestor);

            ValidateChanges();

            return base.SaveChanges();
        }

        public virtual void MarkEntries<T>(DateTime timeStamp, string requestor) {
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

        public virtual void DetachAllEntities() {
            foreach (var entry in ChangeTracker.Entries()) {
                entry.State = EntityState.Detached;
            }
        }
    }
}