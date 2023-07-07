using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using QuizWorld.Infrastructure.Data.Contracts;
using QuizWorld.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.Data.Services.JsonWebToken
{
    /// <summary>
    /// A service to work with JWTs. It allows you to decode JWTs and turn objects into JWTs. It
    /// also grants a limited access to a Redis-based blacklist to invalidate tokens.
    /// </summary>
    public class JwtService : IJwtService
    {

        private readonly IJwtBlacklist blacklist;
        private readonly IConfiguration config;

        public JwtService(IJwtBlacklist blacklist, IConfiguration config)
        {
            this.blacklist = blacklist;
            this.config = config;
        }

        /// <summary>
        /// Checks if the JWT has been blacklisted
        /// </summary>
        /// <param name="jwt">The token to be looked up</param>
        /// <returns>A boolean value that indicates whether the token is blacklisted or not</returns>
        public async Task<bool> CheckIfJWTHasBeenInvalidated(string jwt)
        {
            var token = await this.blacklist.FindJWT(jwt);
            return token != null;
        }

        /// <summary>
        /// Generates a JWT that can be decoded to a UserViewModel
        /// </summary>
        /// <param name="user">The user to be converted to a JWT</param>
        /// <returns>A string representing the JWT</returns>
        public string GenerateJWT(UserViewModel user)
        {
            var secret = this.config["JWT:Secret"];
            var issuer = this.config["JWT:ValidIssuer"];
            var audience = this.config["JWT:ValidAudience"];


            var claims = new List<Claim>
            {
                new Claim("id", user.Id),
                new Claim("username", user.Username),
                new Claim("roles", JsonConvert.SerializeObject(user.Roles))
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.Now.AddDays(1),
                claims: claims,
                signingCredentials:
                    new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Decodes the provided JWT into a UserViewModel.
        /// </summary>
        /// <param name="jwt">The token to be decoded</param>
        /// <returns>A UserViewModel representing the content of the JWT</returns>
        /// <exception cref="InvalidOperationException">If roles is null</exception>

        public UserViewModel DecodeJWT(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            var rolesJson = token.Claims.First(t => t.Type == "roles").Value;
            var roles = JsonConvert.DeserializeObject<string[]>(rolesJson);

            if (roles == null)
            {
                throw new InvalidOperationException("roles is null, this is most likely an error from the token itself");
            }

            var user = new UserViewModel()
            {
                Id = token.Claims.First(t => t.Type == "id").Value,
                Username = token.Claims.First(t => t.Type == "username").Value,
                Roles = roles
            };

            return user;
        }

        /// <summary>
        /// Adds the provided token to the JWT blacklist, rendering it unusable in
        /// future authorized requests
        /// </summary>
        /// <param name="jwt">The token to be blacklisted</param>
        /// <returns>A boolean value that indicates whether the operation succeeded</returns>
        public async Task<bool> InvalidateJWT(string jwt)
        {
            var succeeded = await this.blacklist.BlacklistJWT(jwt);
            return succeeded;
        }

        /// <summary>
        /// Removes the "Bearer " part of the token.
        /// </summary>
        /// <param name="bearerToken">The token to be truncated</param>
        /// <returns>The JWT from the bearer token or an empty string if <paramref name="bearerToken"/> is null</returns>
        public string RemoveBearer(string? bearerToken)
        {
            if (bearerToken == null)
            {
                return string.Empty;
            }

            string jwt = bearerToken.Replace("Bearer ", string.Empty);
            return jwt;
        }
    }
}
