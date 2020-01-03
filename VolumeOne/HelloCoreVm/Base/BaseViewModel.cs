using HelloCoreBll.DataTransferObjects;

namespace HelloCoreVm.Base {
    public class BaseViewModel<T, TId>
        where T : AbstractAuditableBaseDto<TId> {
        public T Auditable { get; set; }
    }
}