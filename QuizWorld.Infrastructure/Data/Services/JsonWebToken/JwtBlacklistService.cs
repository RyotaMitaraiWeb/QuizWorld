﻿using Microsoft.EntityFrameworkCore;
using QuizWorld.Infrastructure.Data.Redis.Models;
using Redis.OM;
using Redis.OM.Searching;

namespace QuizWorld.Infrastructure.Data.Services.JsonwebToken
{
    /// <summary>
    /// A service to manage a Redis-based of JWTs. A blacklisted JWT will be rejected
    /// in any authorized request.
    /// </summary>
    public class JwtBlacklistService
    {
        private readonly RedisConnectionProvider redisProvider;
        private readonly RedisCollection<JWT> tokens;

        public JwtBlacklistService(RedisConnectionProvider redisProvider)
        {
            this.redisProvider = redisProvider;
            tokens = (RedisCollection<JWT>)this.redisProvider.RedisCollection<JWT>();
        }

        /// <summary>
        /// Retrieves a stored JWT in the Redis database
        /// </summary>
        /// <param name="jwt">The JWT to be looked up</param>
        /// <returns>the JWT or null if it doesn't exist</returns>
        public async Task<string?> FindJWT(string jwt)
        {
            var token = await tokens.Where(t => t.Token == jwt).FirstOrDefaultAsync();
            return token?.Token;
        }

        /// <summary>
        /// Inserts the provided JWT in the blacklist. The JWT will be deleted from the
        /// database after 24 hours (at which point the token will have expired)
        /// </summary>
        /// <param name="jwt">The token to be added</param>
        /// <returns>A boolean value indicating whether the operation succeeded</returns>
        public async Task<bool> BlacklistJWT(string jwt)
        {
            var token = await FindJWT(jwt);
            if (token != null)
            {
                return false;
            }

            var result = await tokens.InsertAsync(new JWT
            {
                Id = jwt,
                Token = token
            }, new TimeSpan(24, 0, 0));

            await tokens.SaveAsync();

            return result != null;
        }
    }
}
