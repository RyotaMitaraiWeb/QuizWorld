namespace QuizWorld.Common.Errors
{
    public static class AuthError
    {
        public enum FailedLoginError
        {
            WrongPassword = 1,
            UserDoesNotExist,
        }
    }
}
