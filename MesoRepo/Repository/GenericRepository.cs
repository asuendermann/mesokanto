using Commons.DomainModel.Base;

using DatabaseAccess;

using DomainModel.Administration;
using DomainModel.Base;
using Microsoft.EntityFrameworkCore;

using Repository.SortFilters;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace Repository {
    /// <summary>
    ///     this version of Generic Repository puts the type dependency into the funtion calls.
    ///     Experience in 2 Projects has shown that a GenericRepository instance for each Type produces large overhead in
    ///     Business Layer.
    ///     Even if only one single call is needed, a DB Conection plus a GenericRepository is created.
    ///     With this implemention, all types are seved through one DB Connecion.
    /// </summary>
    public static class GenericRepository {
        /// <summary>
        ///     create an entity in the database, optionally including all child entities.
        ///     Entity itself is checked for UK constraint violation, child entities are not checked.
        ///     This should not be necessary since uniqueness constraints of child entities are usually applicable in the parent
        ///     context only.
        /// </summary>
        /// <typeparam name="T">the type of the entity to be created</typeparam>
        /// <typeparam name="TId">the type of the identity key</typeparam>
        /// <param name="dbAccess">the database context implementation to be extended.</param>
        /// <param name="model">the entity to be created.</param>
        /// <param name="isDuplicate">
        ///     (optional) function to determine if adding the entity would violate the UK constraints.
        ///     Child entities are not checked.
        /// </param>
        /// <returns>a result object indicating the outcome of the operation</returns>
        public static RepositoryResult<T> CreateWithChildren<T, TId>(this BaseDbContext dbAccess, T model,
            Func<T, T, bool> isDuplicate = null)
            where T : BaseAuditable<TId> {
            var result = dbAccess.CheckCreateEntity<T, TId>(model, isDuplicate);

            if (result.Success) {
                var dbSet = dbAccess.Set<T>();
                dbSet.Add(model);

                var changed = dbAccess.SaveChanges();
                var entityAdded = dbAccess.Entry(model).State == EntityState.Unchanged;
                if (0 == changed || !entityAdded) {
                    result.ResultCode |= RepositoryResultCode.SaveFailed;
                }
            }

            if (!result.Success) {
                dbAccess.Entry(model).State = EntityState.Detached;
            }

            return result;
        }

        /// <summary>
        ///     create a single entity in the database. Child entities are not included.
        ///     Entity itself is checked for UK constraint violation.
        /// </summary>
        /// <typeparam name="T">the type of the entity to be created</typeparam>
        /// <typeparam name="TId">the type of the identity key</typeparam>
        /// <param name="dbAccess">the database context implementation to be extended.</param>
        /// <param name="model">the entity to be created.</param>
        /// <param name="isDuplicate">
        ///     (optional) function to determine if adding the entity would violate the UK constraints.
        ///     Child entities are not checked.
        /// </param>
        /// <returns>a result object indicating the outcome of the operation</returns>
        public static RepositoryResult<T> Create<T, TId>(this BaseDbContext dbAccess, T model,
            Func<T, T, bool> isDuplicate)
            where T : BaseAuditable<TId> {
            var result = dbAccess.CheckCreateEntity<T, TId>(model, isDuplicate);

            if (result.Success) {
                dbAccess.Entry(model).State = EntityState.Added;

                var changed = dbAccess.SaveChanges();
                var entityAdded = dbAccess.Entry(model).State == EntityState.Unchanged;
                if (0 == changed || !entityAdded) {
                    result.ResultCode |= RepositoryResultCode.SaveFailed;
                }
            }

            if (!result.Success) {
                dbAccess.Entry(model).State = EntityState.Detached;
            }

            return result;
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

        private static IOrderedQueryable<T> SortQuery<T, TId>(
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

        public static RepositoryResult<T> CheckCreateEntity<T, TId>(
            this BaseDbContext dbAccess,
            T model,
            Func<T, T, bool> isDuplicate = null)
            where T : BaseAuditable<TId> {
            var result = new RepositoryResult<T> {
                Entity = model
            };

            // Check 1: model already has unique key - skip 
            if (!EqualityComparer<TId>.Default.Equals(model.Id, default)) {
                result.ResultCode |= RepositoryResultCode.Impractical;
            }

            // Check 2: model does not pass validation - skip
            dbAccess.ValidateEntity<T, TId>(model, result);

            // Check 3: model violated UK constraint
            var baseAuditables = dbAccess.Set<T>().AsEnumerable() ?? new List<T>();
            if (null != isDuplicate && baseAuditables.Any(q => isDuplicate(q, model))) {
                result.ResultCode |= RepositoryResultCode.Duplicate;
            }

            return result;
        }

        /// <summary>
        ///     local function that checks for duplicates before Create is performed.
        /// </summary>
        /// <param name="a1">first entity to be checked</param>
        /// <param name="a2">second entity to be checked</param>
        /// <returns>true if another entry already uses the specified key false otherwise</returns>
        public static bool CreateComparer(Administrator a1, Administrator a2) {
            return a1.UserIdentityName == a2.UserIdentityName;
        }

        public static bool UpdateComparer(Administrator a1, Administrator a2) {
            return a1.UserIdentityName == a2.UserIdentityName && a1.Id != a2.Id;
        }

        public static void ValidateEntity<T, TId>(this BaseDbContext dbAccess, T entity, RepositoryResult<T> result)
            where T : IAuditable<TId> {
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = dbAccess.Requestor;

            var validationContext = new ValidationContext(entity);
            if (!Validator.TryValidateObject(entity, validationContext, result.ValidationResult, true)) {
                result.ResultCode |= RepositoryResultCode.Invalid;
            }
        }
    }
}