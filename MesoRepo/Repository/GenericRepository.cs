using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Commons.DomainModel.Base;

using DatabaseAccess;

using Microsoft.EntityFrameworkCore;

namespace Repository {
    public class GenericRepository {
        public GenericRepository(BaseDbContext dbAccess) {
            DbAccess = dbAccess;
        }

        private BaseDbContext DbAccess { get; }

        /// <summary>
        /// create an entity in the database. Child objects are not created.
        /// </summary>
        /// <typeparam name="T">the type of the entity to be created</typeparam>
        /// <typeparam name="TId">the type of the identity key</typeparam>
        /// <param name="model">the entity to be created</param>
        /// <param name="isDuplicate">(optional) function to determine if adding the entity would violate the UK constraints</param>
        /// <returns>a result object indicating the outcome of the operation</returns>
        public RepositoryResult<T> Create<T, TId>(T model, Func<T, T, bool> isDuplicate = null)
            where T : BaseAuditable<TId> {

            var result = CheckCreateModel<T, TId>(model, isDuplicate);

            if (result.Success) {
                DbAccess.Entry(model).State = EntityState.Added;
                var changed = DbAccess.SaveChanges();
                var entityAdded = DbAccess.Entry(model).State == EntityState.Unchanged;
                result.SaveFailed = 0 == changed || !entityAdded;
            }

            if (!result.Success) {
                DbAccess.Entry(model).State = EntityState.Detached;
            }
            return result;
        }

        private RepositoryResult<T> CheckCreateModel<T, TId>(T model, Func<T, T, bool> isDuplicate = null)
            where T : BaseAuditable<TId> {
            var result = new RepositoryResult<T> {
                Entity = model
            };

            // Check 1: model already has unique key - skip 
            if (!EqualityComparer<TId>.Default.Equals(model.Id, default)) {
                result.Impractical = true;
            }

            // Check 2: model does not pass validation - skip
            ValidateEntity<T,TId>(model, result);

            // Check 3: check for duplicates and model is duplicate - skip
            var baseAuditables = DbAccess.Set<T>().AsEnumerable() ?? new List<T>();
            if (null != isDuplicate && baseAuditables.Any(q => isDuplicate(q, model))) {
                result.Duplicate = true;
            }

            return result;
        }

        private void ValidateEntity<T, TId>(T entity, RepositoryResult<T> result)
            where T : IAuditable<TId> {
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = DbAccess.Requestor;

            var validationContext = new ValidationContext(entity);
            result.Invalid = !Validator.TryValidateObject(entity, validationContext, result.ValidationResult, true);
        }
    }
}