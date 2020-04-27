namespace Commons.DomainModel.Base {
    /// <summary>
    ///     base interface for Table Per Hierarchy classes.
    ///     Provides a convention for naming of discriminator column.
    /// </summary>
    /// <typeparam name="TId">
    ///     Type of the unique identifier of the database record.
    /// </typeparam>
    public interface ITablePerHierarchy<TId> : IAuditable<TId> {
        string Discriminator { get; set; }
    }
}