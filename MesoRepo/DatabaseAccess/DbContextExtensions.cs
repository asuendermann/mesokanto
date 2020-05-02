using Commons.DomainModel.Base;
using DatabaseAccess.Repository;
using DatabaseAccess.Repository.SortFilters;
using DomainModel.Administration;
using DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DatabaseAccess {
    public static class DbContextExtensions {

        private static string requestor;

        static DbContextExtensions() {
            requestor = typeof(DbContext).Name;
        }

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

        public static void ValidateEntity<T, TId>( T entity, RepositoryResult<T> result)
            where T : IAuditable<TId> {
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = requestor;

            var validationContext = new ValidationContext(entity);
            if (!Validator.TryValidateObject(entity, validationContext, result.ValidationResult, true)) {
                result.ResultCode |= RepositoryResultCode.ValidationFailed;
            }
        }

        public static void MarkEntries<T>(this DbContext dbContext) {
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

        /// <summary>
        ///     create an entity in the database, optionally including all child entities.
        ///     Entity itself is checked for UK constraint violation, child entities are not checked.
        ///     This should not be necessary since uniqueness constraints of child entities are usually applicable in the parent
        ///     context only.
        /// </summary>
        /// <typeparam name="T">the type of the entity to be created</typeparam>
        /// <typeparam name="TId">the type of the identity key</typeparam>
        /// <param name="dbContext">the database context implementation to be extended.</param>
        /// <param name="model">the entity to be created.</param>
        /// <param name="isDuplicate">
        ///     (optional) function to determine if adding the entity would violate the UK constraints.
        ///     Child entities are not checked.
        /// </param>
        /// <returns>a result object indicating the outcome of the operation</returns>
        public static RepositoryResult<T> CreateWithChildren<T, TId>(this DbContext dbContext, T model)
            where T : BaseAuditable<TId> {
            var result = dbContext.CheckCreateEntity<T, TId>(model);

            if (result.Success) {
                var dbSet = dbContext.Set<T>();
                dbSet.Add(model);

                var changed = dbContext.SaveChanges();
                var entityAdded = dbContext.Entry(model).State == EntityState.Unchanged;
                if (0 == changed || !entityAdded) {
                    result.ResultCode |= RepositoryResultCode.SaveFailed;
                }
            }

            if (!result.Success) {
                dbContext.Entry(model).State = EntityState.Detached;
            }

            return result;
        }

        /// <summary>
        ///     create a single entity in the database. Child entities are not included.
        ///     Entity itself is checked for UK constraint violation.
        /// </summary>
        /// <typeparam name="T">the type of the entity to be created</typeparam>
        /// <typeparam name="TId">the type of the identity key</typeparam>
        /// <param name="dbContext">the database context implementation to be extended.</param>
        /// <param name="model">the entity to be created.</param>
        /// <param name="isDuplicate">
        ///     (optional) function to determine if adding the entity would violate the UK constraints.
        ///     Child entities are not checked.
        /// </param>
        /// <returns>a result object indicating the outcome of the operation</returns>
        public static RepositoryResult<T> Create<T, TId>(this BaseDbContext dbContext, T model)
            where T : BaseAuditable<TId> {
            var result = dbContext.CheckCreateEntity<T, TId>(model);

            if (result.Success) {
                dbContext.Entry(model).State = EntityState.Added;

                var changed = dbContext.SaveChanges();
                var entityAdded = dbContext.Entry(model).State == EntityState.Unchanged;
                if (0 == changed || !entityAdded) {
                    result.ResultCode |= RepositoryResultCode.SaveFailed;
                }
            }

            if (!result.Success) {
                dbContext.Entry(model).State = EntityState.Detached;
            }

            return result;
        }

        public static RepositoryResult<T> CheckCreateEntity<T, TId>(this DbContext dbContext,T model)
            where T : BaseAuditable<TId> {
            var result = new RepositoryResult<T> {
                Entity = model
            };

            // Check 1: model already has unique key - skip 
            if (!EqualityComparer<TId>.Default.Equals(model.Id, default)) {
                result.ResultCode |= RepositoryResultCode.Impractical;
            }

            // Check 2: model does not pass validation - skip
            ValidateEntity<T, TId>(model, result);

            // Check 3: model violates UK constraint - skip
            if (model is IUniqueAuditable ) {
                var baseAuditables = dbContext.Set<T>().AsEnumerable() ?? new List<T>();
                var uniqueModel = (IUniqueAuditable)model;
                if (baseAuditables.Any(q => uniqueModel.HasSameUniqueKey(q) ) ) {
                    result.ResultCode |= RepositoryResultCode.Duplicate;
                }
            }

            return result;
        }

        public static void ReadPage<T, TId>(
            this BaseDbContext dbAccess,
            PagedResult<T> result,
            params Expression<Func<T, object>>[] paths)
            where T : BaseAuditable<TId> {
            var queryable = dbAccess.Query<T, TId>(result.Filter, paths);
            var sortedQuery = dbAccess.SortQuery<T, TId>(queryable, result.SortFilters);

            result.RowCount = sortedQuery.Count();

            result.PageCount = (int)Math.Ceiling((double)result.RowCount / result.PageSize);

            var skip = (result.PageNumber - 1) * result.PageSize;
            var results = sortedQuery.Skip(skip).Take(result.PageSize);
            if (null != paths) {
                foreach (var path in paths) {
                    results = results.Include(path);
                }
            }

            result.Results = results;
        }

        public static IQueryable<T> Query<T, TId>(
            this BaseDbContext dbAccess,
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] paths)
            where T : BaseAuditable<TId> {
            var queryable = dbAccess.Set<T>().AsQueryable();

            if (null != predicate) {
                queryable = queryable.Where(predicate);
            }

            if (null != paths) {
                foreach (var path in paths) {
                    queryable = queryable.Include(path);
                }
            }

            return queryable;
        }

        public static IOrderedQueryable<T> SortQuery<T, TId>(
            this BaseDbContext dbAccess,
            IQueryable<T> query,
            ICollection<SortFilter<T>> sortFilters)
            where T : BaseAuditable<TId> {
            var sortedQuery = query.OrderByDescending(t => t.Id);
            if (null == sortFilters) {
                return sortedQuery;
            }

            foreach (var sortFilter in sortFilters) {
                if (sortFilter is SortFilterString<T>) {
                    var expression = ((SortFilterString<T>)sortFilter).Expression;
                    sortedQuery = sortFilter.Descending ?
                        sortedQuery.OrderByDescending(expression) :
                        sortedQuery.OrderBy(expression);
                    return sortedQuery;
                }

                if (sortFilter is SortFilterInt<T>) {
                    var expression = ((SortFilterInt<T>)sortFilter).Expression;
                    sortedQuery = sortFilter.Descending ?
                        sortedQuery.OrderByDescending(expression) :
                        sortedQuery.OrderBy(expression);
                    return sortedQuery;
                }
            }

            return sortedQuery;
        }


    }
}
