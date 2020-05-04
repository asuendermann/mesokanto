using System;

namespace Commons.DomainModel.Base {
    /// <summary>
    /// base interface for all domain model classes.
    /// </summary>
    /// <typeparam name="TId">
    /// Type of the unique identifier of the database record. 
    /// Recommended types are Integer Types:
    /// int (maps on SQL type int) if range is sufficient 
    /// or 
    /// long (maps on SQL type bigint) if large number of records is expected
    /// </typeparam>
    public interface IAuditable<TId> {
        /// <summary>
        /// the unique identifier of the database record.
        /// </summary>
        TId Id { get; set; }

        /// <summary>
        ///     allow tracking of the database entry lifecycle:
        ///     Creation date of the object.
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        ///     allow tracking of the database entry lifecycle:
        ///     Mark the source/origin of the object.
        ///     Must not hold a reference to an identifiable person due to privacy constraints.
        /// </summary>
        string CreatedBy { get; set; }

        /// <summary>
        ///     allow tracking of the database entry lifecycle:
        ///     Last modification date of the object.
        /// </summary>
        DateTime? ModifiedAt { get; set; }

        /// <summary>
        ///     allow tracking of the database entry lifecycle:
        ///     Mark the source/origin of the last modification of the object.
        ///     Must not hold a reference to an identifiable person due to privacy constraints.
        /// </summary>
        string ModifiedBy { get; set; }
    }
}
