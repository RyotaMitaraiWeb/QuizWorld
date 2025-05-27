namespace QuizWorld.Common.Errors
{
    public static class QuizError
    {
        public enum QuizGetError
        {
            DoesNotExist = 1,
            IsDeleted = 2,
        }

        public static readonly Dictionary<QuizGetError, int> QuizGetErrorCodes = new()
        {
            { QuizGetError.DoesNotExist, 1 },
            { QuizGetError.IsDeleted, 1 }, // user should not know which of those two is the reason
        };
    }
}
