namespace QuizWorld.Web.Contracts.Legacy
{
    [Obsolete]
    public interface IJwtBlacklistDeprecated
    {
        public Task<string?> FindJWT(string jwt);
        public Task<bool> BlacklistJWT(string jwt);

    }
}
