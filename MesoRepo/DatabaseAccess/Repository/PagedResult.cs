using DatabaseAccess.Repository.SortFilters;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DatabaseAccess.Repository {
    public class PagedResult<T>
        where T : class {
        public const int PageSize_10 = 10;

        public PagedResult() {
            PageNumber = 1;
            PageSize = PageSize_10;
            Results = new List<T>();
        }

        public Expression<Func<T, bool>> Filter { get; set; }

        public ICollection<SortFilter<T>> SortFilters { get; set; }

        public IEnumerable<T> Results { get; set; }

        public int PageNumber { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public int RowCount { get; set; }
    }
}