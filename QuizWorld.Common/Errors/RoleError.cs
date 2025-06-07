namespace QuizWorld.Common.Errors
{
    public static class RoleError
    {
        public enum AddRoleError
        {
            UserDoesNotExist = 1,
            UserAlreadyHasRole,
        }

        public enum RemoveRoleError
        {
            UserDoesNotExist = 1,
            UserDoesNotHaveRoleInFirstPlace,
        }
    }
}
