namespace QuizWorld.Web.Contracts
{
    public interface IJwtBlacklist
    {
        public Task<string?> FindJWT(string jwt);
        public Task<bool> BlacklistJWT(string jwt);

    }
}
