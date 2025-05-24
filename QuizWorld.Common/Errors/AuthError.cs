namespace QuizWorld.Common.Errors
{
    public static class AuthError
    {
        public enum FailedLoginError
        {
            WrongPassword = 1,
            UserDoesNotExist,
        }

        public enum FailedRegisterError
        {
            UsernameIsTaken = 1,
            Fail = 2,
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
