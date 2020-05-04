namespace DatabaseAccess {
    public static class DbResultCode {
        public const int Success = 0;

        public const int Impractical = 1;

        public const int ValidationFailed = 2;

        public const int Duplicate = 4;

        public const int EntityIsNull = 8;

        public const int SaveFailed = 16;
    }
}