using System;
using System.Linq.Expressions;

namespace DatabaseAccess.SortFilters {
    public class SortFilterString<T> : SortFilter<T>
        where T : class {
        public Expression<Func<T, string>> Expression { get; set; }
    }
}