using System;
using System.Collections.Generic;
using System.Text;

using HelloCoreBll.DataTransferObjects;

using HelloCoreCommons.Paging;

namespace HelloCoreVm.Base {
    public class PagedViewModel<T,TId> : PagedResult<T> where T : AbstractAuditableBaseDto<TId> {
    }
}
