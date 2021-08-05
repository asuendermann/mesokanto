namespace DatabaseAccess.SortFilters {
    public class SortFilter<T>
        where T : class {
        public bool Descending { get; set; }
    }
}