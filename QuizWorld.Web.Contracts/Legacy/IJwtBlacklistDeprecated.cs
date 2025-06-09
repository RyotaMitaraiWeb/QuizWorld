namespace QuizWorld.Web.Contracts.JsonWebToken
{
    [Obsolete]
    public interface IJwtBlacklistDeprecated
    {
        public Task<string?> FindJWT(string jwt);
        public Task<bool> BlacklistJWT(string jwt);

    }
}
