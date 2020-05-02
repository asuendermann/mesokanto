using DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DatabaseAccess {
    public static class DbContextExtensions {
        /// <summary>
        ///     Validation should be done on each layer separately.
        ///     We perform an extensive validation to handle ValidationAttributes set outside
        ///     the context of EF and database. EF Core does not do this anymore (EF6 did).
        /// </summary>
        public static void ValidateChanges(this DbContext dbContext) {
            var errorMessages = new List<string>();
            foreach (var entry in dbContext.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)) {
                var entity = entry.Entity;
                var context = new ValidationContext(entity);
                var results = new List<ValidationResult>();

                if (Validator.TryValidateObject(entity, context, results, true) == false) {
                    foreach (var result in results) {
                        if (result != ValidationResult.Success) {
                            errorMessages.Add(result.ErrorMessage);
                            Log.Warning(result.ErrorMessage);
                        }
                    }
                }
            }

            if (errorMessages.Any()) {
                throw new ValidationException(string.Join(';', errorMessages));
            }
        }

        public static void MarkEntries<T>(this DbContext dbContext, string requestor) {
            var timeStamp = DateTime.UtcNow;
            foreach (var entity in dbContext.ChangeTracker.Entries<BaseAuditable<T>>()) {
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

        public static void DetachAllEntities(this DbContext dbContext) {
            foreach (var entry in dbContext.ChangeTracker.Entries()) {
                entry.State = EntityState.Detached;
            }
        }
    }
}
