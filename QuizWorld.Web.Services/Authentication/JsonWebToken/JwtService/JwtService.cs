using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using QuizWorld.Common.Claims;
using QuizWorld.Common.Result;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts.Authentication.JsonWebToken;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static QuizWorld.Common.Results.JwtError;

namespace QuizWorld.Web.Services.Authentication.JsonWebToken.JwtService
{
    public class JwtService(IJwtStore store, IConfiguration config) : IJwtService
    {
        private readonly IJwtStore _store = store;
        private readonly IConfiguration _config = config;
        private string Secret
        {
            get
            {
                return _config["JWT_SECRET"] ?? string.Empty;
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
    }
}
