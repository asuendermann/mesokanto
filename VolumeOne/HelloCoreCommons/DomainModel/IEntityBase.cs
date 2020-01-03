namespace HelloCoreCommons.DomainModel {

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId">
    /// Type of the unique identifier of the database record. 
    /// Recommended types are Integer Types:
    /// int (maps on SQL type int) if range is sufficient 
    /// or 
    /// long (maps on SQL type bigint) if large number of records is expected
    /// </typeparam>
    public interface IEntityBase<TId> {
        /// <summary>
        /// the unique identifier of the database record.
        /// </summary>
        TId Id { get; set; }

    }
}
