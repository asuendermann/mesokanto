using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DatabaseAccess {
    public class DbResult<T> {
        public DbResult() {
            ValidationResults = new List<ValidationResult>();
        }
        public bool Success => DbResultCode.Success == ResultCode;

        public int ResultCode { get; set; }

        public bool Impractical => DbResultCode.Impractical == (ResultCode & DbResultCode.Impractical);

        public bool Duplicate => DbResultCode.Duplicate == (ResultCode & DbResultCode.Duplicate);

        public bool Invalid => DbResultCode.ValidationFailed == (ResultCode & DbResultCode.ValidationFailed);

        public bool SaveFailed => DbResultCode.SaveFailed == (ResultCode & DbResultCode.SaveFailed);

        public ICollection<ValidationResult> ValidationResults { get; set; }

        public T Entity { get; set; }
    }
}