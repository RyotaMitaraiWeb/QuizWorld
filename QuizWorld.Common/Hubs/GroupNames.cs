namespace QuizWorld.Common.Hubs
{
    public static class GroupNames
    {
        public static string UserIdGroup(string userId) => $"userid-group-{userId.ToLower()}";
    }
}
