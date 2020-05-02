﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DatabaseAccess.Repository {
    public class RepositoryResult<T> {
        public bool Success => RepositoryResultCode.Success == ResultCode;

        public int ResultCode { get; set; }

        public bool Impractical => RepositoryResultCode.Impractical == (ResultCode & RepositoryResultCode.Impractical);

        public bool Duplicate => RepositoryResultCode.Duplicate == (ResultCode & RepositoryResultCode.Duplicate);

        public bool Invalid => RepositoryResultCode.ValidationFailed == (ResultCode & RepositoryResultCode.ValidationFailed);

        public bool SaveFailed => RepositoryResultCode.SaveFailed == (ResultCode & RepositoryResultCode.SaveFailed);

        public ICollection<ValidationResult> ValidationResult { get; set; }

        public T Entity { get; set; }
    }
}