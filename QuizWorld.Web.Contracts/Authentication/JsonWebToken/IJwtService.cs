using QuizWorld.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Web.Contracts.JsonWebToken;

public interface IJwtService
{
    public string GenerateJWT(UserViewModel user);
    public UserViewModel DecodeJWT(string jwt);
    public Task<bool> InvalidateJWT(string jwt);
    public Task<bool> CheckIfJWTHasBeenInvalidated(string jwt);
    public string RemoveBearer(string? bearerToken);
    public Task<bool> CheckIfJWTIsValid(string jwt);
}
