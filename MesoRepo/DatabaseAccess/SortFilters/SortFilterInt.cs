using System;
using System.Linq.Expressions;

namespace Repository.SortFilters {
    public class SortFilterInt<T> : SortFilter<T>
        where T : class {
        public Expression<Func<T, int>> Expression { get; set; }
    }
}