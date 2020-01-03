using HelloCoreCommons.DomainModel;

namespace HelloCoreBll.DataTransferObjects {
    public abstract class AbstractEntityBaseDto<TId> : IEntityBase<TId> {
        public TId Id { get; set; }
    }
}