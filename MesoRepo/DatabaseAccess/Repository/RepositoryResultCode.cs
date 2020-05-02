namespace DatabaseAccess.Repository {
    public static class RepositoryResultCode {
        public const int Success = 0;

        public const int Impractical = 1;

        public const int ValidationFailed = 2;

        public const int Duplicate = 4;

        public const int SaveFailed = 8;
    }
}