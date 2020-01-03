using System;
using System.Collections.Generic;

namespace HelloCoreCommons.Paging {
    public class PagedResult<T> {

        public const int PageSize_10 = 10;
        public const int PageSize_20 = 20;

        public PagedResult() {
            CurrentPageNo = 1;
            PageSize = PageSize_10;
            Results = new List<T>();
        }

        public int CurrentPageNo { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public int RowCount { get; set; }

        public int FirstRowOnPage => (CurrentPageNo - 1) * PageSize + 1;

        public int LastRowOnPage => Math.Min(CurrentPageNo * PageSize, RowCount);

        public IEnumerable<T> Results { get; set; }
    }
}