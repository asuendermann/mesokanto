using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using HelloCoreCommons.DomainModel;
using HelloCoreCommons.Paging;

namespace HelloCoreDal.Repository {
    /// <summary>
    ///     this interface lists a large set of imaginable methods to manipulate and read the contents of a repository.
    /// </summary>
    /// <typeparam name="T">the target type from the Domain Model</typeparam>
    /// <typeparam name="TId">
    ///     the unique identifier type of the target type.
    ///     Recommended types ar int or long which allow for autoincrement in database.
    /// </typeparam>
    public interface IGenericRepository<T, TId>
        where T : IEntityBase<TId> {
        /// <summary>
        ///     create a single domain object in the database.
        /// </summary>
        /// <param name="model">the domain object to be created.</param>
        /// <returns>true if the object has been stored in the database, false otherwise.</returns>
        bool Create(T model);

        /// <summary>
        ///     create a set of domain objects in the database.
        /// </summary>
        /// <param name="models">the domain objects to be created.</param>
        /// <returns>true if all objects have been stored in the database, false otherwise.</returns>
        bool Create(IEnumerable<T> models);

        /// <summary>
        ///     create a single domain object in the database.
        ///     Check if the specified contraint is violated.
        /// </summary>
        /// <param name="predicate">the expression that checks the constraint. if constraint is true, do not create the entry.</param>
        /// <param name="model">the domain object to be created.</param>
        /// <returns>true if the object has been stored in the database, false otherwise.</returns>
        bool CreateWithContraint(Expression<Func<T, bool>> predicate, T model);

        /// <summary>
        ///     update a single domain object in the database.
        /// </summary>
        /// <param name="model">the domain object to be updated.</param>
        /// <returns>true if the object has been updated in the database, false otherwise.</returns>
        bool Update(T model);

        /// <summary>
        ///     update a single domain object in the database. Check if the speified contraint is violated.
        /// </summary>
        /// <param name="predicate">the expression that checks the constraint. if constraint is true, do not create the entry.</param>
        /// <param name="model">the domain object to be created.</param>
        /// <returns>true if the object has been stored in the database, false otherwise.</returns>
        bool UpdateWithContraint(Expression<Func<T, bool>> predicate, T model);

        /// <summary>
        ///     update a set of domain objects in the database.
        /// </summary>
        /// <param name="models">the domain objects to be updated.</param>
        /// <returns>true if all objects have been updated in the database, false otherwise.</returns>
        bool Update(IEnumerable<T> models);

        /// <summary>
        ///     create a single domain object in the database if it is not already present else update the object in the database.
        /// </summary>
        /// <param name="model">the domain object to be created or updated.</param>
        /// <returns>true if the object has been processed in the database, false otherwise.</returns>
        bool CreateOrUpdate(T model);

        /// <summary>
        ///     create elements from a set of domain objects in the database if not already present else update the object in the
        ///     database.
        /// </summary>
        /// <param name="models">the domain objects to be created or updated.</param>
        /// <returns>true if all objects have been processed in the database, false otherwise.</returns>
        bool CreateOrUpdate(IEnumerable<T> models);

        /// <summary>
        ///     find one single entry by its identifier.
        /// </summary>
        /// <param name="id">the unique identifier of the entry.</param>
        /// <returns>the entry for the specfied identifier if present, or null.</returns>
        T Find(TId id);

        /// <summary>
        ///     count all entries of the specified type in the database.
        /// </summary>
        /// <returns>the number entries of the specified type in the database.</returns>
        int Count();

        /// <summary>
        ///     count all entries of the specified type fulfilling the specified condition in the database.
        /// </summary>
        /// <param name="predicate">the condition to be fulfilled</param>
        /// <returns>the number entries of the specified type fulfilling the specified condition in the database.</returns>
        int Count(Expression<Func<T, bool>> predicate);

        /// <summary>
        ///     search for a queryable list of objects in the database.
        /// </summary>
        /// <param name="predicate">the condition to be fulfilled by the objects.</param>
        /// <param name="paths">the include paths for the results</param>
        /// <returns>a queryable list list conatining the matching domain objects.</returns>
        IQueryable<T> Query(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] paths);

        /// <summary>
        ///     search for an enumerable list of domain objects in the database.
        /// </summary>
        /// <param name="paths">the include paths for the results</param>
        /// <returns>an enumerable list containing the all domain objects.</returns>
        IEnumerable<T> Read(params Expression<Func<T, object>>[] paths);

        /// <summary>
        ///     search for an enumerable list of domain objects in the database.
        /// </summary>
        /// <param name="predicate">the condition to be fulfilled by the objects.</param>
        /// <param name="paths">the include paths for the results</param>
        /// <returns>an enumerable list containing the matching domain objects.</returns>
        IEnumerable<T> Read(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] paths);

        /// <summary>
        ///     search for a paged enumerable list domain objects in the database.
        /// </summary>
        /// <param name="currentPageNo">the current page number</param>
        /// <param name="pageSize">the maximum number of entries to be returned on a page</param>
        /// <param name="predicate">the condition to be fulfilled by the objects.</param>
        /// <param name="paths">the include paths for the results</param>
        /// <returns>
        ///     a PagedResult with an enumerable list containing the matching domain objects on the specified page
        ///     and with the specified maximum number of entries. If insufficient entries are available, an empty list is returned.
        /// </returns>
        PagedResult<T> ReadPage(int currentPageNo, int pageSize, Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] paths);

        /// <summary>
        ///     remove a domain object from the database.
        /// </summary>
        /// <param name="model">the domain object to be removed.</param>
        /// <returns>true if the object has been removed from in the database or was not present therein, false otherwise.</returns>
        bool Delete(T model);

        /// <summary>
        ///     remove a set of domain objects from the database.
        /// </summary>
        /// <param name="models">the domain objects to be removed.</param>
        /// <returns>true if all objects have been removed from the database, false otherwise.</returns>
        bool Delete(T[] models);
    }
}