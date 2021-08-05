using System.Collections.Generic;
using System.Linq;

namespace DatabaseAccess {
    public class DbResults<T> {
        public DbResults() {
            Results = new List<DbResult<T>>();
        }

        public bool Success => Results.All(r => r.Success);

        public ICollection<DbResult<T>> Results { get; set; }
    }
}