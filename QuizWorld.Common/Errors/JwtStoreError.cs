namespace QuizWorld.Common.Results
{
    public static class JwtStoreError
    {
        public enum RetrieveTokenError
        {
            NotFound
        }

        public enum BlacklistTokenError
        {
            AlreadyBlacklisted,
        }
    }
}
