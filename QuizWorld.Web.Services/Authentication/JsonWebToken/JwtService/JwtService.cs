using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using QuizWorld.Common.Claims;
using QuizWorld.Common.Result;
using QuizWorld.Infrastructure.Data.Redis.Models;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts.Authentication.JsonWebToken;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static QuizWorld.Common.Results.JwtError;

namespace QuizWorld.Web.Services.Authentication.JsonWebToken.JwtService
{
    public class JwtService(IConfiguration config) : IJwtService
    {
        private readonly IConfiguration _config = config;
        private string Secret
        {
            get
            {
                return _config["JWT_SECRET"] ?? string.Empty;
            }
        }

        private string Issuer
        {
            get
            {
                return _config["JWT_VALID_ISSUER"] ?? string.Empty;
            }
        }

        private string Audience
        {
            get
            {
                return _config["JWT_VALID_AUDIENCE"] ?? string.Empty;
            }
        }
        
        public Result<string, GenerateTokenErrors> GenerateToken(UserViewModel user)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();

                byte[] key = Encoding.ASCII.GetBytes(Secret);
                var credentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                );

                ClaimsIdentity claims = GenerateClaims(user);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = credentials,
                    Audience = Audience,
                    Issuer = Issuer,
                };

                SecurityToken token = handler.CreateToken(tokenDescriptor);
                string jwt = handler.WriteToken(token);

                return Result<string, GenerateTokenErrors>
                    .Success(jwt);
            }
            catch
            {
                return Result<string, GenerateTokenErrors>
                    .Failure(GenerateTokenErrors.Fail);
            }
        }

        public async Task<Result<UserViewModel, ExtractUserFromTokenErrors>> ExtractUserFromTokenAsync(string bearerToken)
        {
            string token = RemoveBearer(bearerToken);
            var handler = new JwtSecurityTokenHandler();

            var tokenValidationResult = await TokenIsValid(handler, token);
            if (tokenValidationResult.IsFailure)
            {
                return Result<UserViewModel, ExtractUserFromTokenErrors>
                    .Failure(tokenValidationResult.Error);
            }

            Dictionary<string, string> claims = tokenValidationResult
                .Value
                .Claims
                .ToDictionary(c => c.Key, c => c.Value.ToString() ?? string.Empty);

            UserViewModel user = ExtractPayload(tokenValidationResult.Value);

            return Result<UserViewModel, ExtractUserFromTokenErrors>
                .Success(user);
        }

        private static string RemoveBearer(string token)
        {
            return token.Replace("Bearer ", string.Empty);
        }

        private static ClaimsIdentity GenerateClaims(UserViewModel user)
        {
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(UserClaimsProperties.Id, user.Id));
            claims.AddClaim(new Claim(UserClaimsProperties.Username, user.Username));
            claims.AddClaim(new Claim(UserClaimsProperties.Roles, JsonConvert.SerializeObject(user.Roles)));

            return claims;
        }

        private async Task<Result<TokenValidationResult, ExtractUserFromTokenErrors>> TokenIsValid(JwtSecurityTokenHandler handler, string jwt)
        {
            try
            {
                byte[] key = Encoding.ASCII.GetBytes(Secret);
                var credentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                );

                TokenValidationResult token = await handler.ValidateTokenAsync(jwt, new TokenValidationParameters()
                {
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,
                    ValidAudience = Audience,
                });

                if (token.IsValid)
                {
                    return Result<TokenValidationResult, ExtractUserFromTokenErrors>
                        .Success(token);
                }

                Console.WriteLine(token.Exception.Message);

                throw token.Exception;
            }
            catch (SecurityTokenExpiredException)
            {
                return Result<TokenValidationResult, ExtractUserFromTokenErrors>
                    .Failure(ExtractUserFromTokenErrors.Expired);
            }
            catch
            {
                return Result<TokenValidationResult, ExtractUserFromTokenErrors>
                    .Failure(ExtractUserFromTokenErrors.Invalid);
            }
        }

        private static UserViewModel ExtractPayload(TokenValidationResult result)
        {
            Dictionary<string, string> claims = result
                .Claims
                .ToDictionary(c => c.Key, c => c.Value.ToString() ?? string.Empty);

            var user = new UserViewModel()
            {
                Id = claims[UserClaimsProperties.Id],
                Username = claims[UserClaimsProperties.Username],

                // this is what the "roles" property is converted to when generating the token
                Roles = JsonConvert.DeserializeObject<string[]>(claims["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]) ?? []
            };

            return user;
        }
    }
}
