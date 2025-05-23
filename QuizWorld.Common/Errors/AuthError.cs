namespace QuizWorld.Common.Errors
{
    public static class AuthError
    {
        public enum FailedLoginError
        {
            WrongPassword = 1,
            UserDoesNotExist,
        }

        /// <summary>
        /// Wrong password and non-existant user intentionally produce the same error code
        /// to prevent information leaks.
        /// </summary>
        public static readonly Dictionary<FailedLoginError, int> FailedLoginErrorCodes = new()
        {
            { FailedLoginError.WrongPassword, 1 },
            { FailedLoginError.UserDoesNotExist, 1 },
        };
    }
}
