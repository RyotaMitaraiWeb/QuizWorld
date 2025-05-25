namespace QuizWorld.Common.Results
{
    public static class JwtError
    {
        public enum GenerateTokenErrors
        {
            Fail = 1,
        }

        public enum ExtractUserFromTokenErrors
        {
            Invalid = 1,
            Expired = 2,
        }
    }
}
