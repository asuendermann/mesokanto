using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

using HelloCoreCommons.DomainModel;
using HelloCoreCommons.Paging;

using HelloCoreDal.DataAccessLayer;

using Microsoft.EntityFrameworkCore;

namespace HelloCoreDal.Repository {
    public class GenericDbRepository<T, TId> : IGenericRepository<T, TId> where T : class, IEntityBase<TId> {
        public GenericDbRepository(DemoDbContext context) {
            Context = context;
        }

        protected DemoDbContext Context { get; }

        public bool Create(T model) {
            if (null == model || !EqualityComparer<TId>.Default.Equals(model.Id, default) || !IsValid(model)) {
                return false;
            }

            Context.Entry(model).State = EntityState.Added;
            Context.SaveChanges();
            var success = Context.Entry(model).State == EntityState.Unchanged;
            return success;
        }

        public bool Create(IEnumerable<T> models) {
            var success = true;
            if (null != models) {
                using (var t = Context.Database.BeginTransaction()) {
                    foreach (var model in models) {
                        success = EqualityComparer<TId>.Default.Equals(model.Id, default);
                        if (!success) {
                            t.Rollback();
                            break;
                        }

                        Context.Entry(model).State = EntityState.Added;
                        success = 1 == Context.SaveChanges();
                        if (!success) {
                            t.Rollback();
                            break;
                        }
                    }

                    t.Commit();
                }
            }

            return success;
        }

        public bool Create(Expression<Func<T, bool>> predicate, T model) {
            if (null == model || !EqualityComparer<TId>.Default.Equals(model.Id, default) || !IsValid(model)) {
                return false;
            }

            if (Context.Set<T>().Any(predicate)) {
                return false;
            }

            Context.Entry(model).State = EntityState.Added;
            Context.SaveChanges();
            var success = Context.Entry(model).State == EntityState.Unchanged;
            return success;
        }

        public bool Update(T model) {
            if (null == model || EqualityComparer<TId>.Default.Equals(model.Id, default) || !IsValid(model)) {
                return false;
            }

            Context.Entry(model).State = EntityState.Modified;
            Context.SaveChanges();
            var success = Context.Entry(model).State == EntityState.Unchanged;
            return success;
        }

        public bool UpdateWithContraint(Expression<Func<T, bool>> predicate, T model) {
            if (null == model || EqualityComparer<TId>.Default.Equals(model.Id, default) || !IsValid(model)) {
                return false;
            }

            if (Context.Set<T>().Any(predicate)) {
                return false;
            }

            Context.Entry(model).State = EntityState.Modified;
            Context.SaveChanges();
            var success = Context.Entry(model).State == EntityState.Unchanged;
            return success;
        }

        public bool Update(IEnumerable<T> models) {
            var success = true;
            if (null != models) {
                using (var t = Context.Database.BeginTransaction()) {
                    foreach (var model in models) {
                        success = !EqualityComparer<TId>.Default.Equals(model.Id, default);
                        if (!success) {
                            t.Rollback();
                            break;
                        }

                        Context.Entry(model).State = EntityState.Modified;
                        success = 1 == Context.SaveChanges();
                        if (!success) {
                            t.Rollback();
                            break;
                        }
                    }

                    t.Commit();
                }
            }

            return success;
        }

        public bool CreateOrUpdate(T model) {
            if (null == model) {
                return false;
            }

            Context.Entry(model).State = EqualityComparer<TId>.Default.Equals(model.Id, default) ?
                EntityState.Added :
                EntityState.Modified;

            Context.SaveChanges();
            var success = Context.Entry(model).State == EntityState.Unchanged;
            return success;
        }

        public bool CreateOrUpdate(IEnumerable<T> models) {
            var success = true;
            if (null != models) {
                using (var t = Context.Database.BeginTransaction()) {
                    foreach (var model in models) {
                        // the comparer is for both key types, string and int
                        Context.Entry(model).State = EqualityComparer<TId>.Default.Equals(model.Id, default) ?
                            EntityState.Added :
                            EntityState.Modified;
                        success = 1 == Context.SaveChanges();
                        if (!success) {
                            t.Rollback();
                            break;
                        }
                    }

                    t.Commit();
                }
            }

            return success;
        }

        public T Find(TId id) {
            return Context.Set<T>().Find(id);
        }

        public int Count() {
            return Context.Set<T>().Count();
        }

        public int Count(Expression<Func<T, bool>> predicate) {
            return Context.Set<T>().Count(predicate);
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] paths) {
            var model = Context.Set<T>().AsQueryable();
            if (null != paths) {
                foreach (var path in paths) {
                    model = model.Include(path);
                }
            }

            return model.Where(predicate).AsNoTracking();
        }

        public IEnumerable<T> Read(params Expression<Func<T, object>>[] paths) {
            var model = Context.Set<T>().AsQueryable();
            if (null != paths) {
                foreach (var path in paths) {
                    model = model.Include(path);
                }
            }

            return model.AsNoTracking().ToList();
        }

        public IEnumerable<T> Read(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] paths) {
            var model = Context.Set<T>().AsQueryable();
            if (null != paths) {
                foreach (var path in paths) {
                    model = model.Include(path);
                }
            }

            return model.Where(predicate).AsNoTracking().ToList();
        }

        public PagedResult<T> ReadPage(
            int currentPageNo,
            int pageSize,
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] paths) {
            var query = Query(predicate, paths);
            var rowCount = query.Count();

            var pageCount = (int) Math.Ceiling((double) rowCount / pageSize);

            var skip = (currentPageNo - 1) * pageSize;
            var results = query.Skip(skip).Take(pageSize).ToList();

            var result = new PagedResult<T> {
                CurrentPageNo = currentPageNo,
                PageSize = pageSize,
                PageCount = pageCount,
                RowCount = rowCount,
                Results = results
            };

            return result;
        }

        public bool Delete(T model) {
            if (null == model) {
                return true;
            }

            Context.Entry(model).State = EntityState.Deleted;
            Context.SaveChanges();
            var success = Context.Entry(model).State == EntityState.Unchanged;
            return success;
        }

        public bool Delete(T[] models) {
            var success = true;
            if (null != models) {
                using (var t = Context.Database.BeginTransaction()) {
                    foreach (var model in models) {
                        success = !EqualityComparer<TId>.Default.Equals(model.Id, default);
                        if (!success) {
                            t.Rollback();
                            break;
                        }

                        Context.Entry(model).State = EntityState.Deleted;
                        success = 1 == Context.SaveChanges();
                        if (!success) {
                            t.Rollback();
                            break;
                        }
                    }

                    t.Commit();
                }
            }

            return success;
        }

        public bool IsValid(T entity) {
            if (typeof(IAuditableBase<TId>).IsAssignableFrom(typeof(T))) {
                var auditable = (IAuditableBase<TId>) entity;
                auditable.CreatedAt = DateTime.UtcNow;
                auditable.CreatedBy = Context.Requestor;
            }

            var context = new ValidationContext(entity);
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(entity, context, results, true);
        }
    }
}