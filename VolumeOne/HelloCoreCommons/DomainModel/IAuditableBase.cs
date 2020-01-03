using System;

namespace HelloCoreCommons.DomainModel {
    public interface IAuditableBase<T> : IEntityBase<T> {
        /// <summary>
        ///     allow tracking of the database entry lifecycle:
        ///     Creation date of the object.
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        ///     allow tracking of the database entry lifecycle:
        ///     Mark the source/origin of the object.
        ///     Must not hold a reference to an identifyable person due to privacy constraints.
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
        ///     Must not hold a reference to an identifyable person due to privacy constraints.
        /// </summary>
        string ModifiedBy { get; set; }
    }
}
