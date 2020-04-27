using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repository {
    public class RepositoryResult<T> {


        public bool Success => !Impractical && !Duplicate && !Invalid && !SaveFailed;

        public bool Impractical { get; set; }

        public bool Duplicate { get; set; }

        public bool Invalid { get; set; }

        public bool SaveFailed { get; set; }


        public ICollection<ValidationResult> ValidationResult { get; set; }

        public T Entity { get; set; }
    }
}