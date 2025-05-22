using QuizWorld.Common.Result;
using static QuizWorld.Common.Results.JwtStoreError;

namespace QuizWorld.Web.Contracts.Authentication.JsonWebToken
{
    /// <summary>
    /// An interface for interacting with databases that track blacklisted JWTs.
    /// Blacklisted JWTs cannot be used in authorized requests, even if they are
    /// otherwise valid
    /// </summary>
    public interface IJwtStore
    {
        /// <summary>
        /// Makes the JWT unusable for future authorized requests
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns>A result containing the blacklisted token</returns>
        Task<Result<string, BlacklistTokenError>> BlacklistTokenAsync(string jwt);

        /// <summary>
        /// Retrieves the token that is stored as is in the database
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns>A result containing <paramref name="jwt"/></returns>
        Task<Result<string, RetrieveTokenError>> RetrieveBlacklistedTokenAsync(string jwt);
    }
}
