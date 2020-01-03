using System.Collections.Generic;

using HelloCoreBll.DataTransferObjects;

namespace HelloCoreVm.Base {
    public class ListViewModel<T, TId>
        where T : AbstractAuditableBaseDto<TId> {
        public ListViewModel() {
            Auditables = new List<T>();
        }

        public IEnumerable<T> Auditables { get; set; }
    }
}