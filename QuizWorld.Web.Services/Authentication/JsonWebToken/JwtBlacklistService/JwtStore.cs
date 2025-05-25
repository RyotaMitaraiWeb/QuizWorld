using QuizWorld.Infrastructure.Data.Redis.Models;
using QuizWorld.Web.Contracts.Authentication.JsonWebToken;
using Redis.OM.Searching;
using Redis.OM;
using static QuizWorld.Common.Results.JwtStoreError;
using QuizWorld.Common.Result;
namespace QuizWorld.Web.Services.Authentication.JsonWebToken.JwtBlacklistService
{
    public class JwtStore(RedisConnectionProvider redisProvider) : IJwtStore
    {
        private readonly RedisCollection<JWT> _tokens = (RedisCollection<JWT>)redisProvider.RedisCollection<JWT>();

        public async Task<Result<string, BlacklistTokenError>> BlacklistTokenAsync(string jwt)
        {
            var retrieveTokenResult = await GetToken(jwt);
            if (retrieveTokenResult.IsSuccess)
            {
                return Result<string, BlacklistTokenError>
                    .Failure(BlacklistTokenError.AlreadyBlacklisted);
            }

            await _tokens.InsertAsync(new JWT
            {
                Id = jwt,

            }, new TimeSpan(24, 0, 0));

            await _tokens.SaveAsync();

            return Result<string, BlacklistTokenError>
                .Success(jwt);
        }

        public Task<Result<string, RetrieveTokenError>> RetrieveBlacklistedTokenAsync(string jwt)
        {
            return GetToken(jwt);
        }

        private async Task<Result<string, RetrieveTokenError>> GetToken(string jwt)
        {
            JWT? token = await this._tokens
                .Where(t => t.Id == jwt)
                .FirstOrDefaultAsync();

            string? tokenId = token?.Id;

            if (tokenId is null)
            {
                return Result<string, RetrieveTokenError>
                    .Failure(RetrieveTokenError.NotFound);
            }

            return Result<string, RetrieveTokenError>
                .Success(tokenId);
        }
    }
}
