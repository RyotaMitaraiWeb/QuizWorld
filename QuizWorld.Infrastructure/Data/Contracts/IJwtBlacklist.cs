namespace QuizWorld.Infrastructure.Data.Contracts
{
    public interface IJwtBlacklist
    {
        public Task<string?> FindJWT(string jwt);
        public Task<bool> BlacklistJWT(string jwt);

    }
}
